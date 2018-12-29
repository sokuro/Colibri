using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    /*
     * Controller for the User Services View
     *
     * authorize only the SuperAdmin
     */
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<ServicesController> _localizer;

        private int PageSize = 5;

        // bind to the UserServices ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public UserServicesViewModel UserServicesViewModel { get; set; }

        // bind to the UserServices ViewModel
        // necessary to create new Objects
        [BindProperty]
        public UserServicesAddToEntityViewModel UserServicesAddToEntityViewModel { get; set; }

        public ServicesController(ColibriDbContext colibriDbContext, 
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<ServicesController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // initialize the Constructor with the UserServicesViewModel
            UserServicesViewModel = new UserServicesViewModel()
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                UserServices = new List<UserServices>(),
                Users = new List<ApplicationUser>()
            };

            // initialize the Constructor with the UserServicesAddToEntityViewModel
            UserServicesAddToEntityViewModel = new UserServicesAddToEntityViewModel()
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                UserServices = new Models.UserServices()
            };
        }

        // Index
        [Route("Admin/UserServices/Index")]
        public async Task<IActionResult> Index(
            int productPage = 1,
            string searchUserName = null,
            string searchServiceName = null)
        {
            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/UserServices/Index?servicePage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchName=");
            if (searchServiceName != null)
            {
                param.Append(searchServiceName);
            }

            // populate the Lists
            UserServicesViewModel.UserServices = await _colibriDbContext.UserServices.ToListAsync();
            UserServicesViewModel.Users = from u in _colibriDbContext.ApplicationUsers
                                                    join p in _colibriDbContext.Products
                                                    .Include(p => p.ApplicationUser)
                                                    .ThenInclude(p => p.UserName)
                                                    on u.Id equals p.ApplicationUserId
                                                    select u;

            // Search Conditions
            if (searchUserName != null)
            {
                UserServicesViewModel.UserServices = UserServicesViewModel.UserServices
                    .Where(a => a.ApplicationUser.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchServiceName != null)
            {
                UserServicesViewModel.UserServices = UserServicesViewModel.UserServices
                    .Where(a => a.Name.ToLower().Contains(searchServiceName.ToLower())).ToList();
            }

            // Pagination
            // count Advertisements alltogether
            var count = UserServicesViewModel.UserServices.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            UserServicesViewModel.UserServices = UserServicesViewModel.UserServices
                                                        .OrderBy(p => p.Name)
                                                        .Skip((productPage - 1) * PageSize)
                                                        .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            UserServicesViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["UserService"] = _localizer["UserServiceText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["NewUserService"] = _localizer["NewUserServiceText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];

            return View(UserServicesViewModel);
        }

        // GET: create a new Service
        // pass the ViewModel for the DropDown Functionality of the Category Types
        [Route("Admin/UserServices/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateUserService"] = _localizer["CreateUserServiceText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["SpecialTag"] = _localizer["SpecialTagText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(UserServicesAddToEntityViewModel);
        }

        // POST: create a new Service
        // ViewModel bound automatically
        [Route("Admin/UserServices/Create")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createPost()
        {
            //// Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            //// Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // add a UserService first to retrieve it, so one can add User Service to it
                _colibriDbContext.Add(UserServicesAddToEntityViewModel.UserServices);
                await _colibriDbContext.SaveChangesAsync();

                // Image being saved
                // use the Hosting Environment
                string webRootPath = _hostingEnvironment.WebRootPath;

                // retrieve all Files (typed by the User in the View )
                var files = HttpContext.Request.Form.Files;

                // to update the User Service from the DB: retrieve the Db Files
                // new Properties will be added to the specific User Service -> Id needed!
                var userServicesFromDb = _colibriDbContext.UserServices.Find(UserServicesAddToEntityViewModel.UserServices.Id);

                // Image File has been uploaded from the View
                if (files.Count != 0)
                {
                    // Image has been uploaded
                    // the exact Location of the ImageFolderProduct for the Service
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderService);

                    // find the Extension of the File
                    var extension = Path.GetExtension(files[0].FileName);

                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, UserServicesAddToEntityViewModel.UserServices.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    userServicesFromDb.Image = @"\" + StaticDetails.ImageFolderService + @"\" + UserServicesAddToEntityViewModel.UserServices.Id + extension;
                }
                // Image File has not been uploaded -> use a default one
                else
                {
                    // a DUMMY Image if the User does not have uploaded any File (default Image)
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderService + @"\" + StaticDetails.DefaultServiceImage);

                    // copy the Image from the Server and rename it as the ProductImage ID
                    System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderService + @"\" + UserServicesAddToEntityViewModel.UserServices.Id + ".jpg");

                    // update the ProductFromDb.Image with the actual FileName
                    userServicesFromDb.Image = @"\" + StaticDetails.ImageFolderService + @"\" + UserServicesAddToEntityViewModel.UserServices.Id + ".jpg";
                }

                // add the current User as the Creator of the Advertisement
                userServicesFromDb.ApplicationUserId = claim.Value;

                // save the Changes asynchronously
                // update the Image Part inside of the DB
                await _colibriDbContext.SaveChangesAsync();

                // Publish the Created Advertisement's Product
                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    Console.WriteLine("Publishing an User Service Message.");
                    Console.WriteLine();

                    //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
                    //bus.Publish(productsFromDb, "my_subscription_id");

                    await bus.SendAsync("create_user_service", userServicesFromDb);
                }

                // TODO
                // Convert to JSON
                //var parsedJson = new JavaScriptSerializer().Serialize(ProductsViewModel);
                var result = Json(UserServicesAddToEntityViewModel);

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(UserServicesAddToEntityViewModel);
            }
        }

        // Get: /<controller>/Edit
        [Route("Admin/UserServices/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes
            UserServicesAddToEntityViewModel.UserServices = await _colibriDbContext.UserServices
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                //.Include(m => m.SpecialTags)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (UserServicesAddToEntityViewModel.UserServices == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditUserService"] = _localizer["EditUserServiceText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the UserServicesAddToEntityViewModel into the View
            return View(UserServicesAddToEntityViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Admin/UserServices/Edit/{id}")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // for uploaded Images
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                // to replace an Image, first remove the old One
                // get the Product from the DB
                var userServiceFromDb = await _colibriDbContext.UserServices.Where(m => m.Id == UserServicesAddToEntityViewModel.UserServices.Id).FirstOrDefaultAsync();
                // does the File exist and was uploaded by the User
                if (files.Count > 0 && files[0] != null)
                {
                    // if the User uploades a new Image
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderService);
                    // find out the Extension of the new Image File and also the Extension of the old Image existing in the DB
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(userServiceFromDb.Image);

                    // delete the old File
                    if (System.IO.File.Exists(Path.Combine(uploads, UserServicesAddToEntityViewModel.UserServices.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, UserServicesAddToEntityViewModel.UserServices.Id + extension_old));
                    }

                    // copy the new File
                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, UserServicesAddToEntityViewModel.UserServices.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    UserServicesAddToEntityViewModel.UserServices.Image = @"\" + StaticDetails.ImageFolderService + @"\" + UserServicesAddToEntityViewModel.UserServices.Id + extension_new;
                }

                /*
                 * update the productsFromDb and save them back into the DB
                 */
                // Image
                if (UserServicesAddToEntityViewModel.UserServices.Image != null)
                {
                    // replace the old Image
                    userServiceFromDb.Image = UserServicesAddToEntityViewModel.UserServices.Image;
                }
                // Name
                userServiceFromDb.Name = UserServicesAddToEntityViewModel.UserServices.Name;
                // Price
                userServiceFromDb.Price = UserServicesAddToEntityViewModel.UserServices.Price;
                // Available
                userServiceFromDb.Available = UserServicesAddToEntityViewModel.UserServices.Available;
                // CategoryTypeId
                userServiceFromDb.CategoryTypeId = UserServicesAddToEntityViewModel.UserServices.CategoryTypeId;
                // SpecialTagId
                //productFromDb.SpecialTagId = ProductsViewModel.Products.SpecialTagId;
                // Description
                userServiceFromDb.Description = UserServicesAddToEntityViewModel.UserServices.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(UserServicesAddToEntityViewModel);
            }
        }

        // GET: Details
        [Route("Admin/UserServices/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual User Service
            var userService = await _colibriDbContext.UserServices
                    .Include(p => p.CategoryGroups)
                    .Include(p => p.CategoryTypes)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // get the User Service's User
            var user = (IEnumerable<ApplicationUser>)(from u in _colibriDbContext.ApplicationUsers
                                                      join p in _colibriDbContext.UserServices
                                                      .Include(p => p.ApplicationUser)
                                                      .ThenInclude(p => p.UserName)
                                                      on u.Id equals p.ApplicationUserId
                                                      select u);

            // add the user as the ApplicationUser to the User Service
            userService.ApplicationUser = user.FirstOrDefault();

            // count the Number of Clicks on the User Service
            userService.NumberOfClicks += 1;

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            // i18n
            ViewData["UserServiceDetails"] = _localizer["UserServiceDetailsText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ContactOwner"] = _localizer["ContactOwnerText"];
            ViewData["RemoveFromBag"] = _localizer["RemoveFromBagText"];
            ViewData["Order"] = _localizer["OrderText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(userService);
        }

        // Details POST
        [Route("Admin/UserServices/Details/{id}")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check first, if anything exists in the Session
            // Session Name : "ssScheduling"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssScheduling");

            // check if null -> create new
            if (lstCartItems == null)
            {
                lstCartItems = new List<int>();
            }

            // add the retrieved Item (id)
            lstCartItems.Add(id);
            // set the Session:
            // Session Name, Value
            HttpContext.Session.Set("ssScheduling", lstCartItems);

            // redirect to Action
            return RedirectToAction("Index", "Services", new { area = "Admin" });
        }

        // Get: /<controller>/Delete
        [Route("Admin/UserServices/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            UserServicesAddToEntityViewModel.UserServices = await _colibriDbContext.UserServices
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (UserServicesAddToEntityViewModel.UserServices == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteUserService"] = _localizer["DeleteUserServiceText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the ProductsViewModel into the View
            return View(UserServicesAddToEntityViewModel);
        }
    }
}