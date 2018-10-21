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

        // Get: /<controller>/Create
        [HttpGet("CategoryTypes/Create")]
        //[Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // Post: /<controller>/Create
        // @param Category
        [HttpPost("CategoryTypes/Create")]
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

        // Get: /<controller>/Edit
        [HttpGet("CategoryTypes/Edit")]
        //[Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryType = await _db.CategoryTypes.FindAsync(id);

            if (categoryType == null)
            {
                return NotFound();
            }
            return View(categoryType);
        }

        // Post: /<controller>/Edit
        // @param Category
        [HttpPost("CategoryTypes/Edit")]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryTypes categoryTypes)
        {
            // the IDs should be equal
            if (id != categoryTypes.Id)
            {
                return NotFound();
            }

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // Update the Changes
                _db.Update(categoryTypes);
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
        public IActionResult Index()
        {
            return View(_db.CategoryTypes.ToList());
        }
    }
}