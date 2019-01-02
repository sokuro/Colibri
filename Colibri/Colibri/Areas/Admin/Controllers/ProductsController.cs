using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
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
     * Controller to handle the Products
     * 
     * authorize only the SuperAdminEndUser
     */
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<ProductsController> _localizer;

        // PageSize (for the Pagination: 5 Appointments/Page)
        private int PageSize = 5;

        // bind to the ViewModel
        // not necessary to create new Objects
        [BindProperty]
        public ProductsViewModel ProductsViewModel { get; set; }

        [BindProperty]
        public ProductsListViewModel ProductsListViewModel { get; set; }

        public ProductsController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<ProductsController> localizer)
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

            // extra for the Index View with the Pagination
            ProductsListViewModel = new ProductsListViewModel()
            {
                Products = new List<Products>(),
                Users = new List<ApplicationUser>()
            };
        }

        // Action Method Create
        [Route("Admin/Products/Index")]
        public async Task<IActionResult> Index(
            int productPage = 1,
            string searchUserName = null,
            string searchProductName = null
            )
        {
            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Products/Index?productPage=:");
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

            // populate the Lists
            ProductsListViewModel.Products = _colibriDbContext.Products.ToList();
            ProductsListViewModel.Users = from u in _colibriDbContext.ApplicationUsers
                                                join p in _colibriDbContext.Products
                                                .Include(p => p.ApplicationUser)
                                                .ThenInclude(p => p.UserName)
                                                on u.Id equals p.ApplicationUserId
                                                select u;

            // Search Conditions
            if (searchUserName != null)
            {
                ProductsListViewModel.Users = ProductsListViewModel.Users
                    .Where(a => a.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchProductName != null)
            {
                ProductsListViewModel.Products = ProductsListViewModel.Products
                    .Where(a => a.Name.ToLower().Contains(searchProductName.ToLower())).ToList();
            }

            // Pagination
            // count Products alltogether
            var count = ProductsListViewModel.Products.Count;

            // Iterate and Filter Products
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            ProductsListViewModel.Products = ProductsListViewModel.Products
                                                    .OrderBy(p => p.Name)
                                                    .Skip((productPage - 1) * PageSize)
                                                    .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            ProductsListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["ProductList"] = _localizer["ProductListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["NewProduct"] = _localizer["NewProductText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["UserName"] = _localizer["UserNameText"];

            return View(ProductsListViewModel);
        }

        // Action Method Create
        // pass the ViewModel for the DropDown Functionality of the Category Types and Special Tags
        [Route("Admin/Products/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateProduct"] = _localizer["CreateProductText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["UserName"] = _localizer["UserNameText"];

            return View(ProductsViewModel);
        }

        // Post: /<controller>/Create
        // ViewModel bound automatically
        [Route("Admin/Products/Create")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                //add a Product first to retrieve it, so one can add Properties to it
                _colibriDbContext.Add(ProductsViewModel.Products);
                await _colibriDbContext.SaveChangesAsync();

                // TODO save in the Search Entity
                //_colibriDbContext.Add(ProductsViewModel.CategoryTypes);
                //await _colibriDbContext.SaveChangesAsync();


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

                // add the CreatedOn Property to the Model
                productsFromDb.CreatedOn = DateTime.Now;

                // save the Changes asynchronously
                // update the Image Part inside of the DB
                await _colibriDbContext.SaveChangesAsync();

                // Publish the Created Product
                //using (var bus = RabbitHutch.CreateBus("host=localhost"))
                //{
                //    //bus.Publish(categoryGroups, "create_category_groups");
                //    await bus.PublishAsync("create_product_by_admin").ContinueWith(task =>
                //    {
                //        if (task.IsCompleted && !task.IsFaulted)
                //        {
                //            Console.WriteLine("Task Completed");
                //            Console.ReadLine();
                //        }
                //    });
                //}

                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    await bus.SendAsync("create_product_by_admin", productsFromDb);
                }


                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ProductsViewModel);
            }
        }

        // Get: /<controller>/Edit
        [Route("Admin/Products/Edit/{id}")]
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
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Admin/Products/Edit/{id}")]
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

        // Get: /<controller>/Details
        [Route("Admin/Products/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
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
            ViewData["ProductDetails"] = _localizer["ProductDetailsText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];
            ViewData["NumberOfProductRates"] = _localizer["NumberOfProductRatesText"];
            ViewData["ProductRating"] = _localizer["ProductRatingText"];
            ViewData["ContactOwner"] = _localizer["ContactOwnerText"];

            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Get: /<controller>/Delete
        [Route("Admin/Products/Delete/{id}")]
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
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Admin/Products/Delete/{id}")]
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
                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    Console.WriteLine("Publishing messages with publish and subscribe.");
                    Console.WriteLine();

                    bus.Publish(products, "removed_products_by_admin");
                }


                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }
    }
}