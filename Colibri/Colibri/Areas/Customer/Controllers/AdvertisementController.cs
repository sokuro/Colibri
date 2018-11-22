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
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };

            // Advertisement ViewModel
            AdvertisementViewModel = new AdvertisementViewModel
            {
                // initialize
                Products = new List<Products>(),

                Users = from u in _colibriDbContext.ApplicationUsers
                        join p in _colibriDbContext.Products
                        on u.Id equals p.ApplicationUserId
                        select u
            };
        }

        // Index
        public async Task<IActionResult> Index(
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

            param.Append("/Customer/Advertisement?productPage=:");
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

            // populate the List
            AdvertisementViewModel.Products = _colibriDbContext.Products.ToList();


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

            // i18n
            ViewData["Advertisement"] = _localizer["AdvertisementText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];

            // return the List of Products
            return View(AdvertisementViewModel);
        }

        // GET: create a new Advertisement
        // pass the ViewModel for the DropDown Functionality of the Category Types
        public IActionResult Create()
        {
            return View(ProductsViewModel);
        }

        // POST: create a new Advertisement
        // ViewModel bound automatically
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual Product
            var product = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // count the Number of Clicks on the Product
            product.NumberOfClicks += 1;

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            return View(product);
        }

        // Details POST
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