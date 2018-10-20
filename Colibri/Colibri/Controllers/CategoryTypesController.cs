using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    public class CategoryTypesController : Controller
    {
        private ColibriDbContext _db;

        public CategoryTypesController(ColibriDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.CategoryTypes.ToList());
        }

        // Post: /<controller>/Create
        // @param Category
        [HttpPost("create")]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryTypes categoryTypes)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                _db.Add(categoryTypes);
                await _db.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(categoryTypes);
            }
        }
    }
}