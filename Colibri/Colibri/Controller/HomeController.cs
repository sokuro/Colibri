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
        private ICategoryData _categoryData;

        // use the ICategoryData Service
        public HomeController(ICategoryData categoryData)
        {
            _categoryData = categoryData;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            // build a new Instance of the DTO
            var model = new HomeIndexViewModel();

            // get Category Information from the Service
            model.Categories = _categoryData.GetAll();

            // render the Model Information
            return View(model);
        }

        // Get: /<controller>/Details
        // @param: Id
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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Post: /<controller>/Create
        // @param category
        [HttpPost]
        public IActionResult Create(CategoryEditModel categoryEditModel)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                var newCategory = new Category();
                newCategory.Name = categoryEditModel.Name;

                newCategory = _categoryData.Add(newCategory);

                return View("Details", newCategory);
            }
            else
            {
                return View();
            }
        }
    }
}
