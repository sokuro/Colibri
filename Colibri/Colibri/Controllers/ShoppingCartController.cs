using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // bind the ShoppingCartViewModel
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public ShoppingCartController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;

            // initialize the ShoppingCartViewModel
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}