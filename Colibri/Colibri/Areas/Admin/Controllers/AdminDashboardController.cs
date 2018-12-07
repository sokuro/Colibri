using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Colibri.Areas.Customer.Controllers;
using Colibri.Data;
using Colibri.Models;
using Colibri.Services;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


namespace Colibri.Controllers
{
    /*
     * Main Dashboard Controller for the Admin
     */
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminDashboardController : Controller
    {
        // private Fields for Object saves
        //private ICategoryTypesData _categoryData;
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<AdvertisementController> _localizer;
        private readonly IEmailSender _emailSender;

        // bind the ViewModel
        [BindProperty]
        public HomeIndexViewModel HomeIndexViewModel { get; set; }

        // CTOR: use the ICategoryData Service
        //public HomeController(ICategoryTypesData categoryData, IEmailSender emailSender)
        public AdminDashboardController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<AdvertisementController> localizer, 
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

        // GET: /<controller>/
        [Route("Admin/AdminDashboard/Index")]
        public IActionResult Index()
        {
            // build a new Instance of the DTO
            //var model = new HomeIndexViewModel();

            // get Category Information from the Category Service
            //model.CategoryTypes = _categoryData.GetAll();

            HomeIndexViewModel.CategoryGroups = HomeIndexViewModel.CategoryGroups
                                                        .OrderBy(c => c.Id);

            HomeIndexViewModel.CategoryTypes = HomeIndexViewModel.CategoryTypes
                                                        .OrderBy(g => g.Id);

            HomeIndexViewModel.Users = HomeIndexViewModel.Users
                                                        .OrderBy(u => u.UserName);

            // render the Model Information
            //return View(model);
            return View(HomeIndexViewModel);
        }

        // GET: Category Groups
        public IActionResult ShowCategoryGroups()
        {
            return View();
        }

        // Get: /<controller>/Details
        // @param: Id (Category)
        [Route("Admin/AdminDashboard/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            // get Category Information from the Service
            //var model = _categoryData.GetById(id);

            // show the Product

            // NullPointer-Exception Handling
            //if (model == null)
            if (id == null)
            {
                //return View("The Category does not exists yet!");
                return NotFound();
            }

            // get the individual Item
            var categoryGroup = await _colibriDbContext.CategoryGroups
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();

            // render the Model Information
            return View(categoryGroup);
        }
    }
}
