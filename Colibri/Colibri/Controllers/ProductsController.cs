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
    public class ProductsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;

        // bind to the ViewModel
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

        public async Task<IActionResult> Index()
        {
            // List of Products
            var products = _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }
    }
}