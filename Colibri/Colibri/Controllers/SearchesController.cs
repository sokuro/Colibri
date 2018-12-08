using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    public class SearchesController : Controller
    {

        private readonly ColibriDbContext _colibriDbContext;

        [BindProperty]
        public SearchViewModel searchVM { get; set; }

        public IActionResult Index()
        {
            return View();
        }
    }
}