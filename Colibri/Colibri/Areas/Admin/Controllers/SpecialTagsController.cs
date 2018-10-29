using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Administrator, PowerUser")]
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        public SpecialTagsController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        // Get: /<controller>/Create
        [HttpGet("SpecialTags/Create")]
        //[Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // Post: /<controller>/Create
        // @param SpecialTag
        [HttpPost("SpecialTags/Create")]
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTags)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                _colibriDbContext.Add(specialTags);
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(specialTags);
            }
        }

        // Get: /<controller>/Edit
        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var specialTag = await _colibriDbContext.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        // Post: /<controller>/Edit
        // @param Category
        [HttpPost("SpecialTags/Edit")]
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTags)
        {
            // the IDs should be equal
            if (id != specialTags.Id)
            {
                return NotFound();
            }

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // Update the Changes
                _colibriDbContext.Update(specialTags);
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                //return View("Details", newCategory);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(specialTags);
            }
        }

        // Get: /<controller>/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var specialTag = await _colibriDbContext.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        // Get: /<controller>/Delete
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var specialTag = await _colibriDbContext.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        // Post: /<controller>/Delete
        // @param Category
        [HttpPost("SpecialTags/Delete"), ActionName("Delete")]
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTag = await _colibriDbContext.SpecialTags.FindAsync(id);

            _colibriDbContext.SpecialTags.Remove(specialTag);

            // Update the Changes
            await _colibriDbContext.SaveChangesAsync();

            // avoid Refreshing the POST Operation -> Redirect
            //return View("Details", newCategory);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            var specialTagsList = _colibriDbContext.SpecialTags.ToList();

            return View(specialTagsList);
        }
    }
}