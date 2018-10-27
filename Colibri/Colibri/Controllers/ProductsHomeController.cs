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
    //[Authorize(Roles = "Administrator, PowerUser")]
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

        // Details View
        public async Task<IActionResult> Details(int id)
        {
            // get the individual Product
            var product = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            return View(product);
        }

        // Details POST
        [HttpPost,ActionName("Details")]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check first, if anything exists in the Session
            // Session Name : "ssSessionOrderExists"
            List<int> lstSessionOrderExists = HttpContext.Session.Get<List<int>>("ssSessionOrderExists");

            // check if null -> create new
            if (lstSessionOrderExists == null)
            {
                lstSessionOrderExists = new List<int>();
            }

            // add the retrieved Item (id)
            lstSessionOrderExists.Add(id);
            // set the Session:
            // Session Name, Value
            HttpContext.Session.Set("ssSessionOrderExists", lstSessionOrderExists);

            // redirect to Action
            return RedirectToAction("Index", "ProductsHome");
        }

        // Remove (from Bag)
        public IActionResult Remove(int id)
        {
            List<int> lstSessionOrderExists = HttpContext.Session.Get<List<int>>("ssSessionOrderExists");

            if (lstSessionOrderExists.Count > 0)
            {
                if (lstSessionOrderExists.Contains(id))
                {
                    // remove the Item (id)
                    lstSessionOrderExists.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("ssSessionOrderExists", lstSessionOrderExists);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }

        // Entry View
        public async Task<IActionResult> Index()
        {
            //var productList = _repository.GetAllProductsAsync();
            var productList = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .ToListAsync();

            return View(productList);
        }
    }
}