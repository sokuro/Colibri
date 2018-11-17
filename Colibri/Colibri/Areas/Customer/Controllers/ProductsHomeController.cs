using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Controllers
{
    /*
     * Controller for the Products View
     */
    [Area("Customer")]
    public class ProductsHomeController : Controller
    {
        //private readonly IColibriRepository _repository;
        private readonly ColibriDbContext _colibriDbContext;

        //public ProductsHomeController(IColibriRepository repository)
        public ProductsHomeController(ColibriDbContext colibriDbContext)
        {
            //_repository = repository;
            _colibriDbContext = colibriDbContext;
        }

        // Entry (Index) View
        public async Task<IActionResult> Index()
        {
            //var productList = _repository.GetAllProductsAsync();
            var productList = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .ToListAsync();

            return View(productList);
        }

        // Details View GET
        public async Task<IActionResult> Details(int id)
        {
            // get the individual Product
            var product = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // count the Number of Clicks on the Product
            product.NumberOfClicks += 1;

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            return View(product);
        }

        // Details POST
        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check first, if anything exists in the Session
            // Session Name : "ssShoppingCart"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            // check if null -> create new
            if (lstCartItems == null)
            {
                lstCartItems = new List<int>();
            }

            // add the retrieved Item (id)
            lstCartItems.Add(id);
            // set the Session:
            // Session Name, Value
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            // redirect to Action
            return RedirectToAction("Index", "ProductsHome", new { area = "Customer" });
        }

        // Remove (from Bag)
        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    // remove the Item (id)
                    lstCartItems.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }

    }
}