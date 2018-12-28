using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Colibri.Areas.Customer.Controllers;
using Colibri.Data;
using Colibri.Models;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


namespace Colibri.Controllers
{
    /*
     * Main HomeController
     * 
     * open without Registering
     */
    public class HomeController : Controller
    {
        // private Fields for Object saves
        //private ICategoryTypesData _categoryData;
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IEmailSender _emailSender;

        // bind the ViewModel
        [BindProperty]
        public HomeIndexViewModel HomeIndexViewModel { get; set; }

        [BindProperty]
        public SearchViewModel searchVM { get; set; }

        // CTOR: use the ICategoryData Service
        //public HomeController(ICategoryTypesData categoryData, IEmailSender emailSender)
        public HomeController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<HomeController> localizer, 
            IEmailSender emailSender)
        {
            // incoming Category Object will be saved into the private Field
            //_categoryData = categoryData;
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;
            _emailSender = emailSender;

            // CTOR initialize
            HomeIndexViewModel = new HomeIndexViewModel
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Users = _colibriDbContext.ApplicationUsers.ToList()
                //Users = new List<ApplicationUser>()
            };

            searchVM = new SearchViewModel()
            {
                Products = new Models.Products(),
                UserServices = new Models.UserServices(),
                //CategoryTypes = new Models.CategoryTypes()
                CategoryTypesList = _colibriDbContext.CategoryTypes.ToList()
            };
        }

        // GET: /<controller>/
        //[Route("Admin/AdminDashboard/Index")]
        public IActionResult Index()
        {
            // i18n
            ViewData["Language"] = _localizer["LanguageText"];
            ViewData["Save"] = _localizer["SaveText"];

            return View(HomeIndexViewModel);
        }

        // GET: /<controller>/Contact
        //[HttpGet("contact")]
        public ActionResult Contact()
        {
            return View();
        }

        // POST: /<controller>/Contact
        //[HttpPost("contact")]
        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Send the notification email
                _emailSender.SendEmailAsync("sokuro@yacrol.com", model.Subject, 
                    $"User {model.Name} with the Email Address {model.Email} sent you this Message: <br/> {model.Message}");

                // display Sent Message
                ViewBag.UserMessage = "Message sent";
                // clear the Model
                ModelState.Clear();
            }
            return View();
        }

        // POST : Action for Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SearchAdvertisement))
            {
                searchVM.SearchAdvertisement = model.SearchAdvertisement;
                RedirectToAction("AdvertisementResults", "Home", new { str = model.SearchAdvertisement });
            }
            return View();
        }

        // GET : Action for AdvertisementResults
        public async Task<IActionResult> AdvertisementResults(SearchViewModel model)
        {
            searchVM.ProductsList = await _colibriDbContext.Products.Where(m => m.Name.Contains(model.SearchAdvertisement)).ToListAsync();
            searchVM.ResultsCounter = searchVM.ProductsList.Count();

            if (searchVM.ResultsCounter < 1)
            {
                searchVM.ArchiveEntryList = await _colibriDbContext.ArchiveEntry.Where(m => m.Name.Contains(model.SearchAdvertisement)).Where(m => m.isOffer == true).ToListAsync();
                searchVM.ResultsCounterArchive = searchVM.ArchiveEntryList.Count();
            }

            return View(searchVM);
        }

        // Handle Layout
        public async Task<IActionResult> LayoutTwo()
        {
            return View();
        }

        // Handle Translation Culture
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
