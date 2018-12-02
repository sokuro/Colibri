using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Areas.Customer.Controllers;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class CategoryGroupsController : Controller
    {
        private ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<CategoryGroupsController> _localizer;

        public CategoryGroupsController(ColibriDbContext colibriDbContext, IStringLocalizer<CategoryGroupsController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;
        }

        [Route("Admin/CategoryGroups/Index")]
        public IActionResult Index()
        {
            var categoryGroupsList = _colibriDbContext.CategoryGroups.ToList();

            // i18n
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["NewCategoryGroup"] = _localizer["NewCategoryGroupText"];

            return View(categoryGroupsList);
        }

        // Get: /<controller>/Create
        [Route("Admin/CategoryGroups/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateCategoryGroup"] = _localizer["CreateCategoryGroupText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

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

                // Publish the Created Category
                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    bus.Publish(categoryGroups, "create_category_groups");
                }


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

            // i18n
            ViewData["EditCategoryGroup"] = _localizer["EditCategoryGroupText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Update"] = _localizer["UpdateText"];

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

            // i18n
            ViewData["DetailsCategoryGroup"] = _localizer["DetailsCategoryGroupText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

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

            // i18n
            ViewData["DeleteCategoryGroup"] = _localizer["DeleteCategoryGroupText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

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

            // Publish the Removed Category
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Publish(categoryGroup, "remove_category_groups");
            }

            // avoid Refreshing the POST Operation -> Redirect
            //return View("Details", newCategory);
            return RedirectToAction(nameof(Index));
        }
    }
}