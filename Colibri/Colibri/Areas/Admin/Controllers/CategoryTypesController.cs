using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using EasyNetQ;
using EasyNetQ.NonGeneric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class CategoryTypesController : Controller
    {
        private ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<CategoryTypesController> _localizer;

        [TempData]
        public string StatusMessage { get; set; }

        public CategoryTypesController(ColibriDbContext colibriDbContext, IStringLocalizer<CategoryTypesController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;
        }

        [Route("Admin/CategoryTypes/Index")]
        public async Task<IActionResult> Index()
        {
            var categoryTypesList = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).ToListAsync();

            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["NewCategoryType"] = _localizer["NewCategoryTypeText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];


            return View(categoryTypesList);
        }

        // Get: /<controller>/Create
        [Route("Admin/CategoryTypes/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

            CategoryTypesAndCategoryGroupsViewModel model = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = new CategoryTypes(),
                CategoryTypesList = _colibriDbContext.CategoryTypes.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
            };

            return View(model);
        }

        // Post: /<controller>/Create
        // @param Category
        [Route("Admin/CategoryTypes/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryTypesAndCategoryGroupsViewModel model)
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // check if CategoryTypes exists or not & check if Combination of CategoryTypes and CategoryGroup exists
                var doesCategoryTypesExist = _colibriDbContext.CategoryTypes.Where(s => s.Name == model.CategoryTypes.Name).Count();
                var doesCategoryTypesAndCategoryGroupsExist = _colibriDbContext.CategoryTypes.Where(s => s.Name == model.CategoryTypes.Name && s.CategoryGroupId == model.CategoryTypes.CategoryGroupId).Count();

                if (doesCategoryTypesExist > 0 && model.isNew)
                {
                    // error
                    StatusMessage = "Error : CategoryTypes Name already exists";
                }
                else
                {
                    if (doesCategoryTypesAndCategoryGroupsExist == 0 && !model.isNew)
                    {
                        // error
                        StatusMessage = "Error : CategoryTypes does not exist";
                    }
                    else
                    {
                        if (doesCategoryTypesAndCategoryGroupsExist > 0)
                        {
                            // error
                            StatusMessage = "Error : CategoryTypes and CategoryGroups combination already exists";
                        }
                        else
                        {
                            // Wenn keine Fehler, Eintrag in DB hinzufügen
                            _colibriDbContext.Add(model.CategoryTypes);
                            await _colibriDbContext.SaveChangesAsync();
                            

                            // Publish the Created Category Type
                            using (var bus = RabbitHutch.CreateBus("host=localhost"))
                            {
                                //bus.Publish(categoryTypes, "create_category_types");
                                await bus.SendAsync("create_category_types", model.CategoryTypes);
                            }

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }

            // If ModelState is not valid
            CategoryTypesAndCategoryGroupsViewModel modelVM = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = model.CategoryTypes,
                CategoryTypesList = _colibriDbContext.CategoryTypes.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }

        // Get: /<controller>/Edit
        [Route("Admin/CategoryTypes/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryType = await _colibriDbContext.CategoryTypes.FindAsync(id);

            if (categoryType == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditCategoryType"] = _localizer["EditCategoryTypeText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Update"] = _localizer["UpdateText"];

            return View(categoryType);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Admin/CategoryTypes/Edit/{id}")]
        [HttpPost]
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
                _colibriDbContext.Update(categoryTypes);
                await _colibriDbContext.SaveChangesAsync();

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

        // Get: /<controller>/Details
        [Route("Admin/CategoryTypes/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryType = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).SingleOrDefaultAsync(m => m.Id == id);

            if (categoryType == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DetailsCategoryType"] = _localizer["DetailsCategoryTypeText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];

            return View(categoryType);
        }

        // Get: /<controller>/Delete
        [Route("Admin/CategoryTypes/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            var categoryType = await _colibriDbContext.CategoryTypes.FindAsync(id);

            if (categoryType == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteCategoryType"] = _localizer["DeleteCategoryTypeText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

            return View(categoryType);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Admin/CategoryTypes/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryType = await _colibriDbContext.CategoryTypes.FindAsync(id);

            _colibriDbContext.CategoryTypes.Remove(categoryType);

            // Update the Changes
            await _colibriDbContext.SaveChangesAsync();

            // avoid Refreshing the POST Operation -> Redirect
            //return View("Details", newCategory);
            return RedirectToAction(nameof(Index));
        }
    }
}