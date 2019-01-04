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
        private readonly IStringLocalizer<AdvertisementOfferController> _localizer;

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
            IStringLocalizer<AdvertisementOfferController> localizer)
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
            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Offers"] = _localizer["OffersText"];
            ViewData["OfferProduct"] = _localizer["OfferProductText"];
            ViewData["OfferUserService"] = _localizer["OfferUserServiceText"];
            ViewData["Products"] = _localizer["ProductsText"];
            ViewData["UserServices"] = _localizer["UserServicesText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Price"] = _localizer["PriceText"];

            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.CurrentUserId = claim.Value;

            // add the current User name as OwnerId of the Advertisement
            AdvertisementViewModel.OwnerId = claimsIdentity.Name;


            // populate Lists in AdvertisementViewModel for specific User
            // Products (Güter)
            AdvertisementViewModel.Products = await _colibriDbContext.Products.Where(s => s.ApplicationUserId.Equals(AdvertisementViewModel.CurrentUserId))
                .Where(m => m.isOffer == true)
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .ToListAsync();

            // UserServices (Dienstleistungen)
            AdvertisementViewModel.UserServices = await _colibriDbContext.UserServices.Where(s => s.ApplicationUserId.Equals(AdvertisementViewModel.CurrentUserId))
                .Where(m => m.isOffer == true)
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .ToListAsync();

            return View(AdvertisementViewModel);
        }

        // GET : Action for CreateProduct
        [Route("Customer/AdvertisementOffer/CreateProduct")]
        public async Task<IActionResult> CreateProduct()
        {
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["OverviewCategories"] = _localizer["OverviewCategoriesText"];

            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.Where(m => m.TypeOfCategoryGroup == "Product").ToListAsync();
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
            AdvertisementViewModel.Product.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            // If ModelState is not valid, return View
            if (!ModelState.IsValid)
            {
                return View(AdvertisementViewModel);
            }

            // add the current User as the Creator of the Advertisement
            AdvertisementViewModel.Product.ApplicationUserId = claim.Value;
            AdvertisementViewModel.Product.ApplicationUserName = claimsIdentity.Name;

            // combine the Advertisement Offer's Date and Time for the DueDateFrom Property
            AdvertisementViewModel.Product.DueDateFrom = AdvertisementViewModel.Product.DueDateFrom
                .AddHours(AdvertisementViewModel.Product.DueTimeFrom.Hour)
                .AddMinutes(AdvertisementViewModel.Product.DueTimeFrom.Minute);

            // combine the Advertisement Offer's Date and Time for the DueDateTo Property
            AdvertisementViewModel.Product.DueDateTo = AdvertisementViewModel.Product.DueDateTo
                .AddHours(AdvertisementViewModel.Product.DueTimeTo.Hour)
                .AddMinutes(AdvertisementViewModel.Product.DueTimeTo.Minute);

            // add timestamp to "CreatedOn"
            AdvertisementViewModel.Product.CreatedOn = System.DateTime.Now;

            // set "available" to TRUE
            AdvertisementViewModel.Product.Available = true;

            // set "isOffer" to TRUE
            AdvertisementViewModel.Product.isOffer = true;

            // If ModelState is valid, save changes to DB
            _colibriDbContext.Products.Add(AdvertisementViewModel.Product);
            await _colibriDbContext.SaveChangesAsync();

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
                if (files[0].Length > 0)
                {
                    // the exact Location of the ImageFolderProduct
                    //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                    var uploads = Path.Combine(webRootPath, "img/ProductImage");

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
                    productsFromDb.Image = @"\img\ProductImage\" + AdvertisementViewModel.Product.Id + extension;
                }
            }
            else
            {
                // a DUMMY Image if the User does not have uploaded any File (default Image)
                //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct + @"\" + StaticDetails.DefaultProductImage);
                var uploads = Path.Combine(webRootPath, @"img\ProductImage\" + StaticDetails.DefaultProductImage);

                // copy the Image from the Server and rename it as the ProductImage ID
                //System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.Product.Id + ".jpg");
                System.IO.File.Copy(uploads, webRootPath + @"\img\ProductImage\" + AdvertisementViewModel.Product.Id + ".jpg");

                // update the ProductFromDb.Image with the actual FileName
                productsFromDb.Image = @"\img\ProductImage\" + AdvertisementViewModel.Product.Id + ".jpg";
            }

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

            // Eintrag für ArchiveEntry erstellen
            ArchiveEntry archiveEntry = new ArchiveEntry()
            {
                Name = AdvertisementViewModel.Product.Name,
                Description = AdvertisementViewModel.Product.Description,
                isOffer = true,
                CategoryTypeId = AdvertisementViewModel.Product.CategoryTypeId,
                CategoryGroupId = AdvertisementViewModel.Product.CategoryGroupId,
                TypeOfCategoryGroup = "Product",
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
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CreateProductOffer"] = _localizer["CreateProductOfferText"];
            ViewData["CreateUserServiceOffer"] = _localizer["CreateUserServiceOfferText"];
            ViewData["OverviewCategories"] = _localizer["OverviewCategoriesText"];

            AdvertisementViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.Where(m => m.TypeOfCategoryGroup == "Service").ToListAsync();
            return View(AdvertisementViewModel);
        }

        // POST : Action for CreateUserService
        [Route("Customer/AdvertisementOffer/CreateUserService")]
        [HttpPost, ActionName("CreateUserService")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserServicePOST()
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
            AdvertisementViewModel.UserService.ApplicationUserName = claimsIdentity.Name;

            // combine the Advertisement Offer's Date and Time for the DueDateFrom Property
            AdvertisementViewModel.UserService.DueDateFrom = AdvertisementViewModel.UserService.DueDateFrom
                .AddHours(AdvertisementViewModel.UserService.DueTimeFrom.Hour)
                .AddMinutes(AdvertisementViewModel.UserService.DueTimeFrom.Minute);

            // combine the Advertisement Offer's Date and Time for the DueDateTo Property
            AdvertisementViewModel.UserService.DueDateTo = AdvertisementViewModel.UserService.DueDateTo
                .AddHours(AdvertisementViewModel.UserService.DueTimeTo.Hour)
                .AddMinutes(AdvertisementViewModel.UserService.DueTimeTo.Minute);

            // add timestamp to "CreatedOn"
            AdvertisementViewModel.UserService.CreatedOn = System.DateTime.Now;

            // set "available" to TRUE
            AdvertisementViewModel.UserService.Available = true;

            // set "isOffer" to TRUE
            AdvertisementViewModel.UserService.isOffer = true;

            // If ModelState is valid, save changes to DB
            _colibriDbContext.UserServices.Add(AdvertisementViewModel.UserService);
            await _colibriDbContext.SaveChangesAsync();

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
            if (files.Count() > 0)
            {
                if (files[0].Length > 0)
                {
                    // the exact Location of the ImageFolderProduct
                    //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct);
                    var uploads = Path.Combine(webRootPath, "img/ServiceImage");

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
                    servicesFromDb.Image = @"\img\ServiceImage\" + AdvertisementViewModel.UserService.Id + extension;
                }
            }
            else
            {
                // a DUMMY Image if the User does not have uploaded any File (default Image)
                //var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolderProduct + @"\" + StaticDetails.DefaultProductImage);
                var uploads = Path.Combine(webRootPath, @"img\ServiceImage\" + StaticDetails.DefaultServiceImage);

                // copy the Image from the Server and rename it as the ProductImage ID
                //System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolderProduct + @"\" + AdvertisementViewModel.Product.Id + ".jpg");
                System.IO.File.Copy(uploads, webRootPath + @"\img\ServiceImage\" + AdvertisementViewModel.UserService.Id + ".jpg");

                // update the ProductFromDb.Image with the actual FileName
                servicesFromDb.Image = @"\img\ServiceImage\" + AdvertisementViewModel.UserService.Id + ".jpg";
            }

            await _colibriDbContext.SaveChangesAsync();

            // Publish the Created Advertisement's Product
            //using (var bus = RabbitHutch.CreateBus("host=localhost"))
            //{
            //    Console.WriteLine("Publishing an Advertisement Message.");
            //    Console.WriteLine();

            //    //bus.Publish<AdvertisementViewModel>(AdvertisementViewModel, "my_subscription_id");
            //    //bus.Publish(productsFromDb, "my_subscription_id");

            //    await bus.SendAsync("create_advertisement", servicesFromDb);
            //}

            // Eintrag für ArchiveEntry erstellen
            ArchiveEntry archiveEntry = new ArchiveEntry()
            {
                Name = AdvertisementViewModel.UserService.Name,
                Description = AdvertisementViewModel.UserService.Description,
                isOffer = true,
                CategoryTypeId = AdvertisementViewModel.UserService.CategoryTypeId,
                CategoryGroupId = AdvertisementViewModel.UserService.CategoryGroupId,
                TypeOfCategoryGroup = "Service",
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
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["DeleteProduct"] = _localizer["DeleteProductText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["Back"] = _localizer["BackText"];

            if (id == null)
            {
                return NotFound();
            }

            var product = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
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

            // Delete Image from DB
            string webRootPath = _hostingEnvironment.WebRootPath;
            var uploads = Path.Combine(webRootPath, "img/ProductImage/");
            var extension_Old = product.Image.Substring(product.Image.LastIndexOf("."), product.Image.Length - product.Image.LastIndexOf("."));


            // delete Image
            try
            {
                    System.IO.File.Delete(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_Old));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

            // Remove product from DB
            _colibriDbContext.Remove(product);

            // Save Changes
            await _colibriDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET : Action for DeleteUserService
        [Route("Customer/AdvertisementOffer/DeleteUserService")]
        public async Task<IActionResult> DeleteUserService(int? id)
        {
            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["DeleteProduct"] = _localizer["DeleteProductText"];
            ViewData["DeleteUserService"] = _localizer["DeleteUserServiceText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["Back"] = _localizer["BackText"];

            if (id == null)
            {
                return NotFound();
            }

            var userService = await _colibriDbContext.UserServices.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            if (userService == null)
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

            // Delete Image from DB
            string webRootPath = _hostingEnvironment.WebRootPath;
            var uploads = Path.Combine(webRootPath, "img/ServiceImage/");
            var extension_Old = userService.Image.Substring(userService.Image.LastIndexOf("."), userService.Image.Length - userService.Image.LastIndexOf("."));

            // delete Image
            try
            {
                System.IO.File.Delete(Path.Combine(uploads, AdvertisementViewModel.UserService.Id + extension_Old));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

            // Remove userService from DB
            _colibriDbContext.Remove(userService);

            // save changes
            await _colibriDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET : Action for EditProduct
        [Route("Customer/AdvertisementOffer/EditProduct")]
        public async Task<IActionResult> EditProduct(int? id)
        {
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["EditProduct"] = _localizer["EditProductText"];
            ViewData["EditUserService"] = _localizer["EditUserServiceText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["Available"] = _localizer["AvailableText"];


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
        public async Task<IActionResult> EditProductPOST(int id)
        {
            // Convert
            AdvertisementViewModel.Product.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            if (id != AdvertisementViewModel.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // combine the Advertisement Offer's Date and Time for the DueDateFrom Property
                    AdvertisementViewModel.Product.DueDateFrom = AdvertisementViewModel.Product.DueDateFrom
                        .AddHours(AdvertisementViewModel.Product.DueTimeFrom.Hour)
                        .AddMinutes(AdvertisementViewModel.Product.DueTimeFrom.Minute);

                    // combine the Advertisement Offer's Date and Time for the DueDateTo Property
                    AdvertisementViewModel.Product.DueDateTo = AdvertisementViewModel.Product.DueDateTo
                        .AddHours(AdvertisementViewModel.Product.DueTimeTo.Hour)
                        .AddMinutes(AdvertisementViewModel.Product.DueTimeTo.Minute);

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
                        var uploads = Path.Combine(webRootPath, "img/ProductImage");
                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
                        var extension_Old = productFromDb.Image.Substring(productFromDb.Image.LastIndexOf("."), productFromDb.Image.Length - productFromDb.Image.LastIndexOf("."));

                        if (System.IO.File.Exists(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_Old));
                        }

                        using (var filestream = new FileStream(Path.Combine(uploads, AdvertisementViewModel.Product.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        AdvertisementViewModel.Product.Image = @"\img\ProductImage\" + AdvertisementViewModel.Product.Id + extension_New;
                    }

                    if (AdvertisementViewModel.Product.Image != null)
                    {
                        productFromDb.Image = AdvertisementViewModel.Product.Image;
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
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
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["EditProduct"] = _localizer["EditProductText"];
            ViewData["EditUserService"] = _localizer["EditUserServiceText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["Available"] = _localizer["AvailableText"];

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
        [HttpPost, ActionName("EditUserService")]
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
                    // combine the Advertisement Offer's Date and Time for the DueDateFrom Property
                    AdvertisementViewModel.UserService.DueDateFrom = AdvertisementViewModel.UserService.DueDateFrom
                        .AddHours(AdvertisementViewModel.UserService.DueTimeFrom.Hour)
                        .AddMinutes(AdvertisementViewModel.UserService.DueTimeFrom.Minute);

                    // combine the Advertisement Offer's Date and Time for the DueDateTo Property
                    AdvertisementViewModel.UserService.DueDateTo = AdvertisementViewModel.UserService.DueDateTo
                        .AddHours(AdvertisementViewModel.UserService.DueTimeTo.Hour)
                        .AddMinutes(AdvertisementViewModel.UserService.DueTimeTo.Minute);

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var userServiceFromDb = _colibriDbContext.UserServices.Where(m => m.Id == AdvertisementViewModel.UserService.Id).FirstOrDefault();

                    userServiceFromDb.Name = AdvertisementViewModel.UserService.Name;
                    userServiceFromDb.Description = AdvertisementViewModel.UserService.Description;
                    userServiceFromDb.CategoryGroupId = AdvertisementViewModel.UserService.CategoryGroupId;
                    userServiceFromDb.CategoryTypeId = AdvertisementViewModel.UserService.CategoryTypeId;
                    userServiceFromDb.Price = AdvertisementViewModel.UserService.Price;
                    userServiceFromDb.DueDateFrom = AdvertisementViewModel.UserService.DueDateFrom;
                    userServiceFromDb.DueDateTo = AdvertisementViewModel.UserService.DueDateTo;
                    userServiceFromDb.Available = AdvertisementViewModel.UserService.Available;

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
                    Console.WriteLine("{0} Exception caught.", ex);
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
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["EditProduct"] = _localizer["EditProductText"];
            ViewData["EditUserService"] = _localizer["EditUserServiceText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Edit"] = _localizer["EditText"];

            if (id == null)
            {
                return NotFound();
            }

            var product = await _colibriDbContext.Products
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET : Action for DetailsUserService
        [Route("Customer/AdvertisementOffer/DetailsUserService")]
        public async Task<IActionResult> DetailsUserService(int? id)
        {
            // i18n
            ViewData["CreateOffer"] = _localizer["CreateOfferText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Image"] = _localizer["ImageText"];
            ViewData["DateFrom"] = _localizer["DateFromText"];
            ViewData["DateTo"] = _localizer["DateToText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["EditProduct"] = _localizer["EditProductText"];
            ViewData["EditUserService"] = _localizer["EditUserServiceText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["Available"] = _localizer["AvailableText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["DetailsProduct"] = _localizer["DetailsProductText"];
            ViewData["DetailsUserService"] = _localizer["DetailsUserServiceText"];

            if (id == null)
            {
                return NotFound();
            }

            var userService = await _colibriDbContext.UserServices
                .Include(m => m.CategoryGroups)
                .Include(m => m.CategoryTypes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userService == null)
            {
                return NotFound();
            }

            return View(userService);
        }

        // GET : Action for Overview
        [Route("Customer/AdvertisementOffer/CategoryOverview")]
        public async Task<IActionResult> CategoryOverview()
        {
            // i18n
            ViewData["Products"] = _localizer["ProductsText"];
            ViewData["UserService"] = _localizer["UserServiceText"];
            ViewData["Overview"] = _localizer["OverviewText"];
            ViewData["OverviewCategories"] = _localizer["OverviewCategoriesText"];
            ViewData["ShowAll"] = _localizer["ShowAllText"];
            ViewData["HideAll"] = _localizer["HideAllText"];

            CategoryTypesAndCategoryGroupsViewModel model = new CategoryTypesAndCategoryGroupsViewModel();

            model.CategoryGroupsList = await _colibriDbContext.CategoryGroups.ToListAsync();
            model.CategoryTypesListE = await _colibriDbContext.CategoryTypes.ToListAsync();

            return View(model);
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