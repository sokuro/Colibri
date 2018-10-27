using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
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

        // Get Index ShoppingCart
        // retrieve all the Products from the Session
        public IActionResult Index()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssSessionOrderExists"
            List<int> lstSessionOrderExists = HttpContext.Session.Get<List<int>>("ssSessionOrderExists");

            if (lstSessionOrderExists.Count > 0)
            {
                foreach (int cartItem in lstSessionOrderExists)
                {
                    
                    Products products = _colibriDbContext.Products
                        .Where(p => p.Id == cartItem)
                        .FirstOrDefault();
                }
            }

            return View();
        }
    }
}