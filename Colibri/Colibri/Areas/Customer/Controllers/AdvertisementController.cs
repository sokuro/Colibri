using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // bind to the ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public AdvertisementViewModel AdvertisementViewModel { get; set; }

        public AdvertisementController(ColibriDbContext colibriDbContext, HostingEnvironment hostingEnvironment)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;

            // initialize the Constructor for the AdvertisementController
            AdvertisementViewModel = new AdvertisementViewModel()
            {
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        // Index
        public async Task<IActionResult> Index()
        {
            var productList = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .ToListAsync();

            return View(productList);
        }

        // GET: create a new Advertisement
        // pass the ViewModel for the DropDown Functionality of the Category Types
        public IActionResult Create()
        {
            return View(AdvertisementViewModel);
        }

        // POST: create a new Advertisement
        // ViewModel bound automatically
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createPost()
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // add a Product first to retrieve it, so one can add Properties to it
                _colibriDbContext.Add(AdvertisementViewModel.Products);
                await _colibriDbContext.SaveChangesAsync();

                // Image being saved
                // use the Hosting Environment
                string webRootPath = _hostingEnvironment.WebRootPath;

                // retrieve all Files (typed by the User in the View )
                var files = HttpContext.Request.Form.Files;

                // to update the Products from the DB: retrieve the Db Files
                // new Properties will be added to the specific Product -> Id needed!
                var productsFromDb = _colibriDbContext.Products.Find(AdvertisementViewModel.Products.Id);

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
                    using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.Products.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    // ProductsImage = exact Path of the Image on the Server + ImageName + Extension
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + AdvertisementViewModel.Products.Id + extension;
                }
                // Image File has not been uploaded -> use a default one
                else
                {
                    // a DUMMY Image if the User does not have uploaded any File (default Image)
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);

                    // copy the Image from the Server and rename it as the ProductImage ID
                    System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + AdvertisementViewModel.Products.Id + ".jpg");

                    // update the ProductFromDb.Image with the actual FileName
                    productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + AdvertisementViewModel.Products.Id + ".jpg";
                }
                // add Special Tags (Id #1 = Offer)
                productsFromDb.SpecialTagId = 1;

                // save the Changes asynchronously
                // update the Image Part inside of the DB
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(AdvertisementViewModel);
            }
        }
    }
}