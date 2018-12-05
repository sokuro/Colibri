using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller for the Advertisement View
     * 
     * authorize only the AdminEndUser (registered User)
     */
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class AdvertisementController : Controller
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

        // bind to the Product ViewModel
        [BindProperty]
        public ProductsViewModel ProductsViewModel { get; set; }

        public AdvertisementController(ColibriDbContext colibriDbContext, HostingEnvironment hostingEnvironment, IStringLocalizer<AdvertisementController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // initialize the Constructor for the ProductsController
            ProductsViewModel = new ProductsViewModel()
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };

            // Advertisement ViewModel
            AdvertisementViewModel = new AdvertisementViewModel
            {
                // initialize
                Products = new List<Products>(),
                Users = new List<ApplicationUser>()
            };
        }

        // Index
        [Route("Customer/Advertisement/Index")]
        public async Task<IActionResult> Index(
            int productPage = 1,
            string searchUserName=null,
            string searchProductName=null,
            string filterMine=null
            )
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/Advertisement/Index?productPage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchName=");
            if (searchProductName != null)
            {
                param.Append(searchProductName);
            }
            param.Append("&searchName=");
            if (filterMine != null)
            {
                param.Append(filterMine);
            }

            // populate the Lists
            AdvertisementViewModel.Products = _colibriDbContext.Products.ToList();
            AdvertisementViewModel.Users = from u in _colibriDbContext.ApplicationUsers
                                                    join p in _colibriDbContext.Products
                                                    .Include(p => p.ApplicationUser)
                                                    .ThenInclude(p => p.UserName)
                                                    on u.Id equals p.ApplicationUserId
                                                    select u;


            // Search Conditions
            if (searchUserName != null)
            {
                AdvertisementViewModel.Products = AdvertisementViewModel.Products
                                                    .Where(a => a.ApplicationUser.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchProductName != null)
            {
                AdvertisementViewModel.Products = AdvertisementViewModel.Products
                                                    .Where(a => a.Name.ToLower().Contains(searchProductName.ToLower())).ToList();
            }
            //if (claim != null)
            //{
            //    filterMine = claim.Value.ToString();
            //    advertisementViewModel.Products = advertisementViewModel.Products
            //                                        .Where(a => a.ApplicationUserId == claim.Value.ToString()).ToList();
            //}

            // Pagination
            // count Advertisements alltogether
            var count = AdvertisementViewModel.Products.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            AdvertisementViewModel.Products = AdvertisementViewModel.Products
                                                    .OrderBy(p => p.Name)
                                                    .Skip((productPage - 1) * PageSize)
                                                    .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            AdvertisementViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["Advertisement"] = _localizer["AdvertisementText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["AdvertisementName"] = _localizer["AdvertisementNameText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["NewAdvertisement"] = _localizer["NewAdvertisementText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];

            // return the List of Products
            return View(AdvertisementViewModel);
        }

        // GET: create a new Advertisement
        // pass the ViewModel for the DropDown Functionality of the Category Types
        [Route("Customer/Advertisement/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateAdvertisement"] = _localizer["CreateAdvertisementText"];
            ViewData["AdvertisementName"] = _localizer["AdvertisementNameText"];
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

            return View(ProductsViewModel);
        }

        // POST: create a new Advertisement
        // ViewModel bound automatically
        [Route("Customer/Advertisement/Create")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createPost()
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // add a Product first to retrieve it, so one can add Properties to it
                _colibriDbContext.Add(ProductsViewModel.Products);
                await _colibriDbContext.SaveChangesAsync();

                // Image being saved
                // use the Hosting Environment
                string webRootPath = _hostingEnvironment.WebRootPath;

                // retrieve all Files (typed by the User in the View )
                var files = HttpContext.Request.Form.Files;

                // to update the Products from the DB: retrieve the Db Files
                // new Properties will be added to the specific Product -> Id needed!
                var productsFromDb = _colibriDbContext.Products.Find(ProductsViewModel.Products.Id);

                // Image File has been uploaded from the View
                if (files.Count != 0)
                {
                    // Image has been uploaded
                    // the exact Location of the ImageFolder
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);

                    // find the Extension of the File
                    var extension = Path.GetExtension(files[0].FileName);

                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsViewModel.Products.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + extension;
                }
                // Image File has not been uploaded -> use a default one
                else
                {
                    // a DUMMY Image if the User does not have uploaded any File (default Image)
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);

                    // copy the Image from the Server and rename it as the ProductImage ID
                    System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg");

                    // update the ProductFromDb.Image with the actual FileName
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg";
                }
                // add Special Tags (Id #1 = Offer)
                // TODO: create a Switch to Offer/Order
                productsFromDb.SpecialTagId = 1;

                // add the current User as the Creator of the Advertisement
                productsFromDb.ApplicationUserId = claim.Value;

                // save the Changes asynchronously
                // update the Image Part inside of the DB
                await _colibriDbContext.SaveChangesAsync();

                // TODO
                // Publish the Created Advertisement's Product
                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    Console.WriteLine("Publishing messages with publish and subscribe.");
                    Console.WriteLine();

                    //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
                    bus.Publish(productsFromDb, "my_subscription_id");
                }

                // TODO
                // Convert to JSON
                //var parsedJson = new JavaScriptSerializer().Serialize(ProductsViewModel);
                var result = Json(ProductsViewModel);

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ProductsViewModel);
            }
        }

        // GET: Details
        [Route("Customer/Advertisement/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual Product
            var product = await _colibriDbContext.Products
                    .Include(p => p.CategoryGroups)
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // get the Product's User
            var user = (IEnumerable<ApplicationUser>)(from u in _colibriDbContext.ApplicationUsers
                                    join p in _colibriDbContext.Products
                                    .Include(p => p.ApplicationUser)
                                    .ThenInclude(p => p.UserName)
                                    on u.Id equals p.ApplicationUserId
                                    select u);

            // add the user as the ApplicationUser to the Product
            product.ApplicationUser = user.FirstOrDefault();

            // count the Number of Clicks on the Product
            product.NumberOfClicks += 1;

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            // i18n
            ViewData["AdvertisementDetails"] = _localizer["AdvertisementDetailsText"];
            ViewData["AdvertisementName"] = _localizer["AdvertisementNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["SpecialTag"] = _localizer["SpecialTagText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ContactOwner"] = _localizer["ContactOwnerText"];
            ViewData["RemoveFromBag"] = _localizer["RemoveFromBagText"];
            ViewData["Order"] = _localizer["OrderText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(product);
        }

        // Details POST
        [Route("Customer/Advertisement/Details/{id}")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check first, if anything exists in the Session
            // Session Name : "ssShoppingCart"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            // check if null -> create new
            if (lstCartItems == null)
            {
                lstCartItems = new List<int>();
            }

            // add the retrieved Item (id)
            lstCartItems.Add(id);
            // set the Session:
            // Session Name, Value
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            // redirect to Action
            return RedirectToAction("Index", "Advertisement", new { area = "Customer" });
        }
    }
}