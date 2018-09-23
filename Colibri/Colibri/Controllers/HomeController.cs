using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Models;
using Colibri.Models.Category;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace Colibri
{
    public class HomeController : Controller
    {
        // private Fields for Object saves
        private ICategoryData _categoryData;
        private IMailService _mailService;

        // CTOR: use the ICategoryData Service
        public HomeController(ICategoryData categoryData, IMailService mailService)
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
            model.Categories = _categoryData.GetAll();

            // render the Model Information
            return View(model);
        }

        // Get: /<controller>/Details
        // @param: Id (Category)
        public IActionResult Details(int id)
        {
            // get Category Information from the Service
            var model = _categoryData.GetById(id);

            // NullPointer-Exception Handling
            if (model == null)
            {
                return View("The Category does not exists yet!");
            }
            // render the Model Information
            return View(model);
        }

        // Get: /<controller>/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // Post: /<controller>/Create
        // @param Category
        [HttpPost("create")]
        public IActionResult Create(CategoryEditModel categoryEditModel)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // copy the Information from the CategoryEditModel into the Category Model
                // save the new Restaurant
                var newCategory = new Category();
                newCategory.Name = categoryEditModel.Name;

                // add the newCategory to the Collection of Categories
                newCategory = _categoryData.Add(newCategory);

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Details), new { id = newCategory.Id });
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View();
            }
        }
    }
}
