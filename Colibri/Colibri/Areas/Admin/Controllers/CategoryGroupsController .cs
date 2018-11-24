using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Admin.Controllers
{
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class CategoryGroupsController : Controller
    {
        private ColibriDbContext _colibriDbContext;

        public CategoryGroupsController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        [Route("Admin/CategoryGroups/Index")]
        public IActionResult Index()
        {
            var categoryGroupsList = _colibriDbContext.CategoryGroups.ToList();

            return View(categoryGroupsList);
        }

        // Get: /<controller>/Create
        [Route("Admin/CategoryGroups/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // Post: /<controller>/Create
        // @param Category
        [Route("Admin/CategoryGroups/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryGroups categoryGroups)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                _colibriDbContext.Add(categoryGroups);
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect

                return RedirectToAction(nameof(Index));
                //return RedirectToAction("Index", "CategoryGroups", new { area = "Admin" });
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(categoryGroups);
            }
        }

        // Get: /<controller>/Edit
        [Route("Admin/CategoryGroups/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryGroup = await _colibriDbContext.CategoryGroups.FindAsync(id);

            if (categoryGroup == null)
            {
                return NotFound();
            }
            return View(categoryGroup);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Admin/CategoryGroups/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryGroups categoryGroups)
        {
            // the IDs should be equal
            if (id != categoryGroups.Id)
            {
                return NotFound();
            }

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // Update the Changes
                _colibriDbContext.Update(categoryGroups);
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(categoryGroups);
            }
        }

        // Get: /<controller>/Details
        [Route("Admin/CategoryGroups/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryGroup = await _colibriDbContext.CategoryGroups.FindAsync(id);

            if (categoryGroup == null)
            {
                return NotFound();
            }
            return View(categoryGroup);
        }

        // Get: /<controller>/Delete
        [Route("Admin/CategoryGroups/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryGroup = await _colibriDbContext.CategoryGroups.FindAsync(id);

            if (categoryGroup == null)
            {
                return NotFound();
            }
            return View(categoryGroup);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Admin/CategoryGroups/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryGroup = await _colibriDbContext.CategoryGroups.FindAsync(id);

            _colibriDbContext.CategoryGroups.Remove(categoryGroup);

            // Update the Changes
            await _colibriDbContext.SaveChangesAsync();

            // avoid Refreshing the POST Operation -> Redirect
            //return View("Details", newCategory);
            return RedirectToAction(nameof(Index));
        }
    }
}