using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // bind to the ViewModel
        [BindProperty]
        public ProductsViewModel ProductsViewModel { get; set; }

        public ProductsController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;

            // initialize the Constructor for the ProductsController
            ProductsViewModel = new ProductsViewModel()
            {
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            // List of Products
            var products = _colibriDbContext.Products.Include(m => m.CategoryTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }
    }
}