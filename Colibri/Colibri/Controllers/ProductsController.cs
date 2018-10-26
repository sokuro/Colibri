using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Controllers
{
    //[Authorize(Roles = "Administrator, PowerUser")]
    public class ProductsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;

        // bind to the ViewModel
        // not necessary to create new Objects
        [BindProperty]
        public ProductsViewModel ProductsViewModel { get; set; }

        public ProductsController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;

            // initialize the Constructor for the ProductsController
            ProductsViewModel = new ProductsViewModel()
            {
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        // Action Method Create
        // pass the ViewModel for the DropDown Functionality of the Category Types and Special Tags
        [HttpGet("Products/Create")]
        //[Authorize]
        public IActionResult Create()
        {
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Create
        // ViewModel bound automatically
        [HttpPost("Products/Create"),ActionName("Create")]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                //add a Product first to retrieve it
                _colibriDbContext.Add(ProductsViewModel.Products);
                await _colibriDbContext.SaveChangesAsync();

                // Image being saved
                // use the Hosting Environment
                string webRootPath = _hostingEnvironment.WebRootPath;
                
                // retrieve all Files
                var files = HttpContext.Request.Form.Files;

                // to update the Products from the DB: retrieve the Db Files
                var productsFromDb = _colibriDbContext.Products.Find(ProductsViewModel.Products.Id);

                // File has been uploaded from the View
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
                // File has not been uploaded
                else
                {
                    // a DUMMY Image if the User does not have uploaded any File (default Image)
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);

                    // copy the Image from the Server and rename it as the ProductImage ID
                    System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg");

                    // update the ProductFromDb.Image with the actual FileName
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg";
                }
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

        // Get: /<controller>/Edit
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsViewModel.Products = await _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsViewModel.Products == null)
            {
                return NotFound();
            }
            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [HttpPost, ActionName("Edit")]
        //[Authorize]
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
                var productFromDb = _colibriDbContext.Products.Where(m => m.Id == ProductsViewModel.Products.Id).FirstOrDefault();
                // does the File exist and was uploaded by the User
                if (files.Count > 0 && files[0] != null)
                {
                    // if the User uploades a new Image
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
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
                    ProductsViewModel.Products.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsViewModel.Products.Id + extension_new;
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
                productFromDb.SpecialTagId = ProductsViewModel.Products.SpecialTagId;
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
        //[Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsViewModel.Products = await _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsViewModel.Products == null)
            {
                return NotFound();
            }
            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Get: /<controller>/Delete
        //[Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsViewModel.Products = await _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsViewModel.Products == null)
            {
                return NotFound();
            }
            // send the ProductsViewModel into the View
            return View(ProductsViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [HttpPost, ActionName("Delete")]
        //[Authorize]
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
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
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

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Index()
        {
            // List of Products
            var productList = await _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(productList);
        }
    }
}