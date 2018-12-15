using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Customer.Controllers
{
    /*
 * Controller for the Advertisement View
 * 
 * authorize only the AdminEndUser (registered User)
 */
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class AdvertisementOfferController : Controller
    {

        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<AdvertisementController> _localizer;

        // PageSize (for the Pagination: 5 Appointments/Page)
        private int PageSize = 4;

        // bind to the Advertisement ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public AdvertisementViewModel AdvertisementViewModel { get; set; }

        // Constructor
        public AdvertisementOfferController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<AdvertisementController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // Advertisement ViewModel
            AdvertisementViewModel = new AdvertisementViewModel()
            {
                // initialize
                Product = new Products(),
                UserService = new UserServices(),
            };
        }


        // GET : Action for Index
        [Route("Customer/AdvertisementOffer/Index")]
        public async Task<IActionResult> Index(int productPage = 1, string searchUserName = null,
            string searchProductName = null, string filterMine = null)
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.CurrentUserId = claim.Value;

            // populate Lists in AdvertisementViewModel for specific User
            // Products (Güter)
            AdvertisementViewModel.Products = await _colibriDbContext.Products.Where(s => s.ApplicationUserId.Equals(AdvertisementViewModel.CurrentUserId))
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .ToListAsync();
            
            // UserServices (Dienstleistungen)
            AdvertisementViewModel.UserServices = await _colibriDbContext.UserServices.Where(s => s.ApplicationUserId.Equals(AdvertisementViewModel.CurrentUserId))
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .ToListAsync();

            //AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();
            //AdvertisementViewModel.CategoryTypes = await _colibriDbContext.CategoryTypes.ToListAsync();
            //AdvertisementViewModel.Products = await _colibriDbContext.Products.ToListAsync();
            //AdvertisementViewModel.UserServices = await _colibriDbContext.UserServices.ToListAsync();

            return View(AdvertisementViewModel);
        }

        // GET : Action for CreateProduct
        [Route("Customer/AdvertisementOffer/CreateProduct")]
        public async Task <IActionResult> CreateProduct()
        {
            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();
            return View(AdvertisementViewModel);
        }

        // POST : Action for CreateProduct
        [Route("Customer/AdvertisementOffer/CreateProduct")]
        [HttpPost, ActionName("CreateProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductPOST()
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Convert
            //AdvertisementViewModel.Product.CategoryGroupId = Convert.ToInt32(Request.Form["CategoryGroup"].ToString());
            AdvertisementViewModel.Product.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            // If ModelState is not valid, return View
            if (!ModelState.IsValid)
            {
                return View(AdvertisementViewModel);
            }

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.Product.ApplicationUserId = claim.Value;

            // add timestamp to "CreatedOn"
            AdvertisementViewModel.Product.CreatedOn = System.DateTime.Now;

            // set "available" to TRUE
            AdvertisementViewModel.Product.Available = true;

            // set "isOffer" to TRUE
            AdvertisementViewModel.Product.isOffer = true;

            // If ModelState is valid, save changes to DB
            _colibriDbContext.Products.Add(AdvertisementViewModel.Product);
            await _colibriDbContext.SaveChangesAsync();

            // TO-DO: Save Changes to Corporate Memory ("Archive")


            // Save Image 
            // use the Hosting Environment
            string webRootPath = _hostingEnvironment.WebRootPath;

            // retrieve all Files (typed by the User in the View )
            var files = HttpContext.Request.Form.Files;

            // to update the Products from the DB: retrieve the Db Files
            // new Properties will be added to the specific Product -> Id needed!
            var productsFromDb = _colibriDbContext.Products.Find(AdvertisementViewModel.Product.Id);

            // Image File has been uploaded from the View
            //if (files != null)
            if (files.Count() > 0)
            {
                if(files[0].Length > 0)
                {
                // the exact Location of the ImageFolderProduct
                //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                var uploads = Path.Combine(webRootPath, "images");

                // find the Extension of the File
                //var extension = Path.GetExtension(files[0].FileName);
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                // use the FileStreamObject -> copy the File from the Uploaded to the Server
                // create the File on the Server
                using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                //productsFromDb.Image = @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.Product.Id + extension;
                productsFromDb.Image = @"\images\" + AdvertisementViewModel.Product.Id + extension;
                }
            }
            else
            {
                // a DUMMY Image if the User does not have uploaded any File (default Image)
                //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct + @"\" + StaticDetails.DefaultProductImage);
                var uploads = Path.Combine(webRootPath, @"img\ProductImage\" + StaticDetails.DefaultProductImage);

                // copy the Image from the Server and rename it as the ProductImage ID
                //System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.Product.Id + ".jpg");
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + AdvertisementViewModel.Product.Id + ".jpg");

                // update the ProductFromDb.Image with the actual FileName
                productsFromDb.Image = @"\images\" + AdvertisementViewModel.Product.Id + ".jpg";
            }

            await _colibriDbContext.SaveChangesAsync();

            // Publish the Created Advertisement's Product
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                Console.WriteLine("Publishing an Advertisement Message.");
                Console.WriteLine();

                //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
                //bus.Publish(productsFromDb, "my_subscription_id");

                await bus.SendAsync("create_advertisement", productsFromDb);
            }

            // Eintrag für ArchiveEntry erstellen
            ArchiveEntry archiveEntry = new ArchiveEntry()
            {
                Name = AdvertisementViewModel.Product.Name,
                Description = AdvertisementViewModel.Product.Description,
                TypeOfAdvertisement = "Offer",
                CategoryTypes = await _colibriDbContext.CategoryTypes.FirstOrDefaultAsync(m => m.Id == AdvertisementViewModel.Product.CategoryTypeId),
                CategoryGroups = await _colibriDbContext.CategoryGroups.FirstOrDefaultAsync(m => m.Id == AdvertisementViewModel.Product.CategoryGroupId),
                CreatedOn = System.DateTime.Now
            };

            // Eintrag in ArchiveEntry-DB schreiben
            _colibriDbContext.ArchiveEntry.Add(archiveEntry);
            await _colibriDbContext.SaveChangesAsync();


            // Zurück zu Index-View
            return RedirectToAction(nameof(Index));
        }


        // GET : Action for CreateUserService
        [Route("Customer/AdvertisementOffer/CreateUserService")]
        public async Task<IActionResult> CreateUserService()
        {
            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();
            return View(AdvertisementViewModel);
        }

        // POST : Action for CreateUserService
        [Route("Customer/AdvertisementOffer/CreateUserService")]
        [HttpPost, ActionName("CreateUserService")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserServicePOST ()
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Convert
            //AdvertisementViewModel.UserService.CategoryGroupId = Convert.ToInt32(Request.Form["CategoryGroup"].ToString());
            AdvertisementViewModel.UserService.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            // If ModelState is not valid, return View
            if (!ModelState.IsValid)
            {
                return View(AdvertisementViewModel);
            }

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.UserService.ApplicationUserId = claim.Value;

            // add timestamp to "CreatedOn"
            AdvertisementViewModel.UserService.CreatedOn = System.DateTime.Now;

            // set "available" to TRUE
            AdvertisementViewModel.UserService.Available = true;

            // If ModelState is valid, save changes to DB
            _colibriDbContext.UserServices.Add(AdvertisementViewModel.UserService);
            await _colibriDbContext.SaveChangesAsync();

            // TO-DO: Save Changes to Corporate Memory ("Archive")


            // Save Image 
            // use the Hosting Environment
            string webRootPath = _hostingEnvironment.WebRootPath;

            // retrieve all Files (typed by the User in the View )
            var files = HttpContext.Request.Form.Files;

            // to update the Products from the DB: retrieve the Db Files
            // new Properties will be added to the specific Product -> Id needed!
            var servicesFromDb = _colibriDbContext.UserServices.Find(AdvertisementViewModel.UserService.Id);

            // Image File has been uploaded from the View
            //if (files != null)
            if(files.Count()>0)
            {
                if (files[0].Length > 0)
                {
                    // the exact Location of the ImageFolderProduct
                    //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                    var uploads = Path.Combine(webRootPath, "images");

                    // find the Extension of the File
                    //var extension = Path.GetExtension(files[0].FileName);
                    var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.UserService.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    //productsFromDb.Image = @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.UserService.Id + extension;
                    servicesFromDb.Image = @"\images\" + AdvertisementViewModel.UserService.Id + extension;
                }
            }
            else
            {
                // a DUMMY Image if the User does not have uploaded any File (default Image)
                //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct + @"\" + StaticDetails.DefaultProductImage);
                var uploads = Path.Combine(webRootPath, @"img\ProductImage\" + StaticDetails.DefaultProductImage);

                // copy the Image from the Server and rename it as the ProductImage ID
                //System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.Product.Id + ".jpg");
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + AdvertisementViewModel.UserService.Id + ".jpg");

                // update the ProductFromDb.Image with the actual FileName
                servicesFromDb.Image = @"\images\" + AdvertisementViewModel.UserService.Id + ".jpg";
            }

            await _colibriDbContext.SaveChangesAsync();

            // Publish the Created Advertisement's Product
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                Console.WriteLine("Publishing an Advertisement Message.");
                Console.WriteLine();

                //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
                //bus.Publish(productsFromDb, "my_subscription_id");

                await bus.SendAsync("create_advertisement", servicesFromDb);
            }

            // Eintrag für ArchiveEntry erstellen
            ArchiveEntry archiveEntry = new ArchiveEntry()
            {
                Name = AdvertisementViewModel.UserService.Name,
                Description = AdvertisementViewModel.UserService.Description,
                TypeOfAdvertisement = "Offer",
                CategoryTypes = await _colibriDbContext.CategoryTypes.FirstOrDefaultAsync(m => m.Id == AdvertisementViewModel.UserService.CategoryTypeId),
                CategoryGroups = await _colibriDbContext.CategoryGroups.FirstOrDefaultAsync(m => m.Id == AdvertisementViewModel.UserService.CategoryGroupId),
                CreatedOn = System.DateTime.Now
            };

            // Eintrag in ArchiveEntry-DB schreiben
            _colibriDbContext.ArchiveEntry.Add(archiveEntry);
            await _colibriDbContext.SaveChangesAsync();

            // Zurück zu Index-View
            return RedirectToAction(nameof(Index));
        }

        // GET : Action for DeleteProduct
        [Route("Customer/AdvertisementOffer/DeleteProduct")]
        public async Task<IActionResult> DeleteProduct (int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var product = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST : Action for DeleteProduct
        [Route("Customer/AdvertisementOffer/DeleteProduct")]
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductPOST(int id)
        {
            var product = await _colibriDbContext.Products.SingleOrDefaultAsync(m => m.Id == id);
            _colibriDbContext.Remove(product);
            await _colibriDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET : Action for DeleteUserService
        [Route("Customer/AdvertisementOffer/DeleteUserService")]
        public async Task<IActionResult> DeleteUserService (int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var userService = await _colibriDbContext.UserServices.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            if(userService == null)
            {
                return NotFound();
            }
            return View(userService);
        }

        // POST : Action for DeleteUserService
        [Route("Customer/AdvertisementOffer/DeleteUserService")]
        [HttpPost, ActionName("DeleteUserService")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserServicePOST(int id)
        {
            var userService = await _colibriDbContext.UserServices.SingleOrDefaultAsync(m => m.Id == id);
            _colibriDbContext.Remove(userService);
            await _colibriDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET : Action for EditProduct
        [Route("Customer/AdvertisementOffer/EditProduct")]
        public async Task<IActionResult> EditProduct(int? id)
        {
            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            AdvertisementViewModel.Product = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            AdvertisementViewModel.CategoryTypes = _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == AdvertisementViewModel.Product.CategoryGroupId).ToList();
            if (AdvertisementViewModel.Product == null)
            {
                return NotFound();
            }
            return View(AdvertisementViewModel);
        }

        // POST : Action for EditProduct
        [Route("Customer/AdvertisementOffer/EditProduct")]
        [HttpPost, ActionName("EditProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductPOST (int id)
        {
            // Convert
            AdvertisementViewModel.Product.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            if(id != AdvertisementViewModel.Product.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var productFromDb = _colibriDbContext.Products.Where(m => m.Id == AdvertisementViewModel.Product.Id).FirstOrDefault();

                    productFromDb.Name = AdvertisementViewModel.Product.Name;
                    productFromDb.Description = AdvertisementViewModel.Product.Description;
                    productFromDb.CategoryGroupId = AdvertisementViewModel.Product.CategoryGroupId;
                    productFromDb.CategoryTypeId = AdvertisementViewModel.Product.CategoryTypeId;
                    productFromDb.Price = AdvertisementViewModel.Product.Price;
                    productFromDb.DueDateFrom = AdvertisementViewModel.Product.DueDateFrom;
                    productFromDb.DueDateTo = AdvertisementViewModel.Product.DueDateTo;
                    productFromDb.Available = AdvertisementViewModel.Product.Available;

                    await _colibriDbContext.SaveChangesAsync();

                    if (files[0].Length > 0 && files[0] != null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
                        var extension_Old = productFromDb.Image.Substring(productFromDb.Image.LastIndexOf("."), productFromDb.Image.Length - productFromDb.Image.LastIndexOf("."));

                        if(System.IO.File.Exists(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_Old));
                        }

                        using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        AdvertisementViewModel.Product.Image = @"\images\" + AdvertisementViewModel.Product.Id + extension_New;
                    }

                    if(AdvertisementViewModel.Product.Image != null)
                    {
                        productFromDb.Image = AdvertisementViewModel.Product.Image;
                    }


                }
                catch(Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is not valid
            AdvertisementViewModel.CategoryTypes = _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == AdvertisementViewModel.Product.CategoryGroupId).ToList();
            return View(AdvertisementViewModel);
        }


        // GET : Action for EditUserService
        [Route("Customer/AdvertisementOffer/EditUserService")]
        public async Task<IActionResult> EditUserService(int? id)
        {
            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            AdvertisementViewModel.UserService = await _colibriDbContext.UserServices.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            AdvertisementViewModel.CategoryTypes = _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == AdvertisementViewModel.UserService.CategoryGroupId).ToList();
            if (AdvertisementViewModel.UserService == null)
            {
                return NotFound();
            }
            return View(AdvertisementViewModel);
        }


        // POST : Action for EditUserService
        [Route("Customer/AdvertisementOffer/EditUserService")]
        [HttpPost, ActionName("EditProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserServicePOST(int id)
        {
            // Convert
            AdvertisementViewModel.UserService.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            if (id != AdvertisementViewModel.UserService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var userServiceFromDb = _colibriDbContext.Products.Where(m => m.Id == AdvertisementViewModel.Product.Id).FirstOrDefault();

                    userServiceFromDb.Name = AdvertisementViewModel.Product.Name;
                    userServiceFromDb.Description = AdvertisementViewModel.Product.Description;
                    userServiceFromDb.CategoryGroupId = AdvertisementViewModel.Product.CategoryGroupId;
                    userServiceFromDb.CategoryTypeId = AdvertisementViewModel.Product.CategoryTypeId;
                    userServiceFromDb.Price = AdvertisementViewModel.Product.Price;
                    userServiceFromDb.DueDateFrom = AdvertisementViewModel.Product.DueDateFrom;
                    userServiceFromDb.DueDateTo = AdvertisementViewModel.Product.DueDateTo;
                    userServiceFromDb.Available = AdvertisementViewModel.Product.Available;

                    await _colibriDbContext.SaveChangesAsync();

                    if (files[0].Length > 0 && files[0] != null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
                        var extension_Old = userServiceFromDb.Image.Substring(userServiceFromDb.Image.LastIndexOf("."), userServiceFromDb.Image.Length - userServiceFromDb.Image.LastIndexOf("."));

                        if (System.IO.File.Exists(Path.Combine(uploads, AdvertisementViewModel.UserService.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, AdvertisementViewModel.UserService.Id + extension_Old));
                        }

                        using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.UserService.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        AdvertisementViewModel.UserService.Image = @"\images\" + AdvertisementViewModel.UserService.Id + extension_New;
                    }

                    if (AdvertisementViewModel.UserService.Image != null)
                    {
                        userServiceFromDb.Image = AdvertisementViewModel.UserService.Image;
                    }


                }
                catch (Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is not valid
            AdvertisementViewModel.CategoryTypes = _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == AdvertisementViewModel.UserService.CategoryGroupId).ToList();
            return View(AdvertisementViewModel);
        }


        // GET : Action for DetailsProduct
        [Route("Customer/AdvertisementOffer/DetailsProduct")]
        public async Task<IActionResult> DetailsProduct(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var product = await _colibriDbContext.Products
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET : Action for DetailsUserService
        [Route("Customer/AdvertisementOffer/DetailsUserService")]
        public async Task<IActionResult> DetailsUserService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userService = await _colibriDbContext.UserServices.FirstOrDefaultAsync(m => m.Id == id);
            if (userService == null)
            {
                return NotFound();
            }

            return View(userService);
        }


        [Route("Customer/AdvertisementOffer/CreateProduct/GetCategory")]
        public JsonResult GetCategory(int CategoryGroupID)
        {
            List<CategoryTypes> categoryTypesList = new List<CategoryTypes>();

            categoryTypesList = (from category in _colibriDbContext.CategoryTypes
                                 where category.CategoryGroupId == CategoryGroupID
                                 select category).ToList();

            return Json(new SelectList(categoryTypesList, "Id", "Name"));
        }

    }
}