using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssSessionOrderExists"
            List<int> lstSessionOrderExists = HttpContext.Session.Get<List<int>>("ssSessionOrderExists");

            if (lstSessionOrderExists.Count > 0)
            {
                foreach (int cartItem in lstSessionOrderExists)
                {
                    // get the Products from the DB
                    // use the eager Method
                    Products products = _colibriDbContext.Products
                        .Include(p => p.CategoryTypes)
                        .Include(p => p.SpecialTags)
                        .Where(p => p.Id == cartItem)
                        .FirstOrDefault();

                    // add the Products to the Shopping Cart
                    ShoppingCartViewModel.Products.Add(products);
                }
            }
            
            // pass the ShoppingCartViewModel to the View
            return View(ShoppingCartViewModel);
        }
    }
}