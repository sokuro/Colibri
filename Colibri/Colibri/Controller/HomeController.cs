using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Models;
using Colibri.Services;
using Microsoft.AspNetCore.Mvc;


namespace Colibri
{
    public class HomeController : Controller
    {
        private ICategoryData _categoryData;

        public HomeController(ICategoryData categoryData)
        {
            _categoryData = categoryData;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new Category { Id = 1, Name = "Produkte " };

            return new ObjectResult(model);
        }
    }
}
