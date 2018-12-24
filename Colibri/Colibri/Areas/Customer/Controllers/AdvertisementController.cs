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

        public AdvertisementController(ColibriDbContext colibriDbContext, 
            HostingEnvironment hostingEnvironment, 
            IStringLocalizer<AdvertisementController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // initialize the Constructor for the ProductsController
            ProductsViewModel = new ProductsViewModel()
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
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

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.CurrentUserId = claim.Value;

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
                //AdvertisementViewModel.Products = AdvertisementViewModel.Products
                //                                    .Where(a => a.ApplicationUser.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
                AdvertisementViewModel.Users = AdvertisementViewModel.Users
                                                    .Where(a => a.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchProductName != null)
            {
                AdvertisementViewModel.Products = AdvertisementViewModel.Products
                                                    .Where(a => a.Name.ToLower().Contains(searchProductName.ToLower())).ToList();
            }
            //if (claim != null)
            if (filterMine != null)
            {
                //filterMine = claim.Value.ToString();
                AdvertisementViewModel.Products = AdvertisementViewModel.Products
                                                    .Where(a => a.ApplicationUserId == AdvertisementViewModel.CurrentUserId).ToList();
            }

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
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["AdvertisementName"] = _localizer["AdvertisementNameText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["NewAdvertisement"] = _localizer["NewAdvertisementText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["ShowMine"] = _localizer["ShowMineText"];

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
                    // the exact Location of the ImageFolderProduct
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);

                    // find the Extension of the File
                    var extension = Path.GetExtension(files[0].FileName);

                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsViewModel.Products.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolderProduct + @"\" + ProductsViewModel.Products.Id + extension;
                }
                // Image File has not been uploaded -> use a default one
                else
                {
                    // a DUMMY Image if the User does not have uploaded any File (default Image)
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct + @"\" + StaticDetails.DefaultProductImage);

                    // copy the Image from the Server and rename it as the ProductImage ID
                    System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderProduct + @"\" + ProductsViewModel.Products.Id + ".jpg");

                    // update the ProductFromDb.Image with the actual FileName
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolderProduct + @"\" + ProductsViewModel.Products.Id + ".jpg";
                }
                // add Special Tags (Id #1 = Offer)
                // TODO: create a Switch to Offer/Order
                //productsFromDb.SpecialTagId = 1;

                // add the current User as the Creator of the Advertisement
                productsFromDb.ApplicationUserId = claim.Value;
                productsFromDb.ApplicationUserName = claim.Subject.Name;

                // save the Changes asynchronously
                // update the Image Part inside of the DB
                await _colibriDbContext.SaveChangesAsync();

                // Publish the Created Advertisement's Product
                //using (var bus = RabbitHutch.CreateBus("host=localhost"))
                //{
                //    Console.WriteLine("Publishing an Advertisement Message.");
                //    Console.WriteLine();

                //    //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
                //    //bus.Publish(productsFromDb, "my_subscription_id");

                //    await bus.SendAsync("create_advertisement", productsFromDb);
                //}

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
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // get the Product's User
            var user = (IEnumerable<ApplicationUser>)(from u in _colibriDbContext.ApplicationUsers
                                    join p in _colibriDbContext.Products
                                    .Include(p => p.ApplicationUser)
                                    .ThenInclude(p => p.UserName)
                                    on u.Id equals p.ApplicationUserId
                                    select u);

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
            ViewData["ProductRating"] = _localizer["ProductRatingText"];
            ViewData["RateProduct"] = _localizer["RateProductText"];
            ViewData["NumberOfProductRates"] = _localizer["NumberOfProductRatesText"];

            return View(product);
        }

        // Handle Ratings: GET
        [Route("Customer/Advertisement/RateAdvertisement/{id}")]
        public async Task<IActionResult> RateAdvertisement(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual Product
            ProductsViewModel.Products = await _colibriDbContext.Products
                    .Include(p => p.CategoryGroups)
                    .Include(p => p.CategoryTypes)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            // i18n
            ViewData["RateQuestion"] = _localizer["RateQuestionText"];
            ViewData["Save"] = _localizer["SaveText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["ProductRating"] = _localizer["ProductRatingText"];
            ViewData["RateProduct"] = _localizer["RateProductText"];

            return View(ProductsViewModel);
        }


        [Route("Customer/Advertisement/RateAdvertisement/{id}")]
        [HttpPost, ActionName("RateAdvertisement")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RateAdvertisementPost(int id, string command)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // to overwrite a Rating, first get the old One
                // get the Product from the DB
                var productFromDb = await _colibriDbContext.Products
                                        .Where(m => m.Id == id)
                                        .FirstOrDefaultAsync();

                int tempProductRating = 0;

                if (command.Equals("1"))
                {
                    tempProductRating = 1;
                }
                else if (command.Equals("2"))
                {
                    tempProductRating = 2;
                }
                else if (command.Equals("3"))
                {
                    tempProductRating = 3;
                }
                else if (command.Equals("4"))
                {
                    tempProductRating = 4;
                }
                else if (command.Equals("5"))
                {
                    tempProductRating = 5;
                }

                // calculate the new ProductRating
                if (productFromDb.NumberOfProductRates == 0)
                {
                    productFromDb.ProductRating = tempProductRating;
                }
                else
                {
                    productFromDb.ProductRating = Math.Round((productFromDb.ProductRating * productFromDb.NumberOfProductRates + tempProductRating) / (productFromDb.NumberOfProductRates + 1), 2);
                }

                // increment the Number of Product Rates of the Product
                productFromDb.NumberOfProductRates += 1;

                // save the Changes in DB
                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Details));
                //return View();
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ProductsViewModel);
            }
        }

        // Details POST
        [Route("Customer/Advertisement/Details/{id}")]
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
            return RedirectToAction("Index", "Scheduling", new { area = "Customer" });
        }

        // Get: /<controller>/Edit
        [Route("Customer/Advertisement/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsViewModel.Products = await _colibriDbContext.Products
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                //.Include(m => m.SpecialTags)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsViewModel.Products == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditProduct"] = _localizer["EditProductText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["SpecialTag"] = _localizer["SpecialTagText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Customer/Advertisement/Edit/{id}")]
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
                var productFromDb = await _colibriDbContext.Products.Where(m => m.Id == ProductsViewModel.Products.Id).FirstOrDefaultAsync();
                // does the File exist and was uploaded by the User
                if (files.Count > 0 && files[0] != null)
                {
                    // if the User uploades a new Image
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                    // find out the Extension of the new Image File and also the Extension of the old Image existing in the DB
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    // delete the old File
                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_old));
                    }

                    // copy the new File
                    // use the FileStreamObject -> copy the File from the Uploaded to the Server
                    // create the File on the Server
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    ProductsViewModel.Products.Image = @"\" + StaticDetails.ImageFolderProduct + @"\" + ProductsViewModel.Products.Id + extension_new;
                }

                /*
                 * update the productsFromDb and save them back into the DB
                 */
                // Image
                if (ProductsViewModel.Products.Image != null)
                {
                    // replace the old Image
                    productFromDb.Image = ProductsViewModel.Products.Image;
                }
                // Name
                productFromDb.Name = ProductsViewModel.Products.Name;
                // Price
                productFromDb.Price = ProductsViewModel.Products.Price;
                // Available
                productFromDb.Available = ProductsViewModel.Products.Available;
                // CategoryTypeId
                productFromDb.CategoryTypeId = ProductsViewModel.Products.CategoryTypeId;
                // SpecialTagId
                //productFromDb.SpecialTagId = ProductsViewModel.Products.SpecialTagId;
                // Description
                productFromDb.Description = ProductsViewModel.Products.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ProductsViewModel);
            }
        }

        // Get: /<controller>/Delete
        [Route("Customer/Advertisement/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsViewModel.Products = await _colibriDbContext.Products
                                                .Include(m => m.CategoryGroups)
                                                .Include(m => m.CategoryTypes)
                                                //.Include(m => m.SpecialTags)
                                                .SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsViewModel.Products == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteProduct"] = _localizer["DeleteProductText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["SpecialTag"] = _localizer["SpecialTagText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Customer/Advertisement/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // find the webRootPath
            string webRootPath = _hostingEnvironment.WebRootPath;
            // find the Product by it's ID
            Products products = await _colibriDbContext.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                // find the Image File
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                // find the Extension
                var extension = Path.GetExtension(products.Image);
                // exists the File?
                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extension)))
                {
                    // remove the File
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }

                // remove the Entry from the DB
                _colibriDbContext.Products.Remove(products);

                // save the Changes asynchronously
                await _colibriDbContext.SaveChangesAsync();


                // TODO
                // Publish the Created Advertisement's Product
                //using (var bus = RabbitHutch.CreateBus("host=localhost"))
                //{
                //    Console.WriteLine("Publishing messages with publish and subscribe.");
                //    Console.WriteLine();

                //    bus.Publish(products, "removed_products_by_admin");
                //}


                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }

        // Remove (from Bag)
        [Route("Customer/Advertisement/Remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssScheduling");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    // remove the Item (id)
                    lstCartItems.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("ssScheduling", lstCartItems);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }
    }
}