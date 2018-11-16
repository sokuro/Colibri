using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Colibri.Models;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        // CTOR: use the ICategoryData Service
        public HomeController(ICategoryTypesData categoryData, IEmailSender emailSender)
        {
            // incoming Category Object will be saved into the private Field
            _categoryData = categoryData;
            _emailSender = emailSender;
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
    }
}
