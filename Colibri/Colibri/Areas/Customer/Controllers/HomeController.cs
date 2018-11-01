using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Models;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Colibri
{
    /*
     * Main HomeController
     * 
     * open without Registering
     */
    [Area("Customer")]
    public class HomeController : Controller
    {
        // private Fields for Object saves
        private ICategoryTypesData _categoryData;
        private IMailService _mailService;

        // CTOR: use the ICategoryData Service
        public HomeController(ICategoryTypesData categoryData, IMailService mailService)
        {
            // incoming Category Object will be saved into the private Field
            _categoryData = categoryData;
            _mailService = mailService;
        }

        // GET: /<controller>/Contact
        [HttpGet("contact")]
        public ActionResult Contact()
        {
            return View();
        }

        // POST: /<controller>/Contact
        [HttpPost("contact")]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Send the notification email
                _mailService.SendMessage("test@test.com", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");
                ViewBag.UserMessage = "Message sent";
                ModelState.Clear();
            }
            return View();
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            // build a new Instance of the DTO
            var model = new HomeIndexViewModel();

            // get Category Information from the Category Service
            model.CategoryTypes = _categoryData.GetAll();

            // render the Model Information
            return View(model);
        }

        // Get: /<controller>/Details
        // @param: Id (Category)
        public IActionResult Details(int id)
        {
            // get Category Information from the Service
            var model = _categoryData.GetById(id);

            // show the Product

            // NullPointer-Exception Handling
            if (model == null)
            {
                return View("The Category does not exists yet!");
            }
            // render the Model Information
            return View(model);
        }

        // Show some Product Offers
        // authorize
        public IActionResult ShowProductOffers()
        {
            return View();
        }
    }
}
