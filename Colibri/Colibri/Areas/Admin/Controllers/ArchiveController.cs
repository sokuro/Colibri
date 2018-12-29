using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class ArchiveController : Controller
    {
        private ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<ArchiveController> _localizer;

        // bind to the Archive ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public ArchiveViewModel ArchiveViewModel { get; set; }

        // Constructor
        public ArchiveController(ColibriDbContext colibriDbContext, IStringLocalizer<ArchiveController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            // ViewModel
            ArchiveViewModel = new ArchiveViewModel()
            {
                ArchiveEntry = new ArchiveEntry()
            };
        }

        // GET : Action for Index
        [Route("Admin/Archive/Index")]
        public async Task<IActionResult> Index()
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            ArchiveViewModel.ArchiveEntryList = await _colibriDbContext.ArchiveEntry.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();

            return View(ArchiveViewModel);
        }

        // GET : Action for Create
        [Route("Admin/Archive/Create")]
        public async Task<IActionResult> Create()
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            ArchiveViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();
            return View(ArchiveViewModel);
        }


        // POST : Action for Create
        [Route("Admin/Archive/Create")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            // Convert
            ArchiveViewModel.ArchiveEntry.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            // If ModelState is not valid, return View
            if (!ModelState.IsValid)
            {
                return View(ArchiveViewModel);
            }

            // add timestamp to "CreatedOn"
            ArchiveViewModel.ArchiveEntry.CreatedOn = System.DateTime.Now;

            // save changes to DB
            _colibriDbContext.ArchiveEntry.Add(ArchiveViewModel.ArchiveEntry);
            await _colibriDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET : Action for Edit
        [Route("Admin/Archive/Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            ArchiveViewModel.CategoryGroups = await _colibriDbContext.CategoryGroups.ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            ArchiveViewModel.ArchiveEntry = await _colibriDbContext.ArchiveEntry.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            ArchiveViewModel.CategoryTypes = await _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == ArchiveViewModel.ArchiveEntry.CategoryGroupId).ToListAsync();

            if(ArchiveViewModel.ArchiveEntry == null)
            {
                return NotFound();
            }

            return View(ArchiveViewModel);
        }

        // POST : Action for Edit
        [Route("Admin/Archive/Edit")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int id)
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            // Convert
            ArchiveViewModel.ArchiveEntry.CategoryTypeId = Convert.ToInt32(Request.Form["CategoryTypeId"].ToString());

            ArchiveViewModel.ArchiveEntry = await _colibriDbContext.ArchiveEntry.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            ArchiveViewModel.CategoryTypes = await _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == ArchiveViewModel.ArchiveEntry.CategoryGroupId).ToListAsync();

            if (id != ArchiveViewModel.ArchiveEntry.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                var entryFromDb = _colibriDbContext.ArchiveEntry.Where(m => m.Id == ArchiveViewModel.ArchiveEntry.Id).FirstOrDefault();

                entryFromDb.Name = ArchiveViewModel.ArchiveEntry.Name;
                entryFromDb.CategoryGroupId = ArchiveViewModel.ArchiveEntry.CategoryGroupId;
                entryFromDb.CategoryTypeId = ArchiveViewModel.ArchiveEntry.CategoryTypeId;
                entryFromDb.isOffer = ArchiveViewModel.ArchiveEntry.isOffer;
                entryFromDb.TypeOfCategoryGroup = ArchiveViewModel.ArchiveEntry.CategoryGroups.TypeOfCategoryGroup;

                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid
            ArchiveViewModel.CategoryTypes = _colibriDbContext.CategoryTypes.Where(s => s.CategoryGroupId == ArchiveViewModel.ArchiveEntry.CategoryGroupId).ToList();
            return View(ArchiveViewModel);
        }


        // GET : Action for Delete
        [Route("Admin/Archive/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            // i18n
            ViewData["ArchiveEntries"] = _localizer["ArchiveEntriesText"];
            ViewData["Back"] = _localizer["BackText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["CreateEntry"] = _localizer["CreateEntryText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["EditEntry"] = _localizer["EditEntryText"];
            ViewData["IsOffer"] = _localizer["IsOfferText"];
            ViewData["NewEntry"] = _localizer["NewEntryText"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["TypeOfCategoryGroup"] = _localizer["TypeOfCategoryGroupText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["DeleteEntry"] = _localizer["DeleteEntryText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            if (id == null)
            {
                return NotFound();
            }

            var entry = await _colibriDbContext.ArchiveEntry.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).SingleOrDefaultAsync(m => m.Id == id);
            if(entry == null)
            {
                return NotFound();
            }
            return View(entry);
        }

        // POST : Action for Delete
        [Route("Admin/Archive/Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var entry = await _colibriDbContext.ArchiveEntry.SingleOrDefaultAsync(m => m.Id == id);

            // remove entry from archive
            _colibriDbContext.Remove(entry);

            // save changes
            await _colibriDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Get Category 
        [Route("Admin/Archive/Edit/GetCategory")]
        public JsonResult GetCategory(int CategoryGroupID)
        {
            List<CategoryTypes> categoryTypesList = new List<CategoryTypes>();

            categoryTypesList = (from category in _colibriDbContext.CategoryTypes
                                 where category.CategoryGroupId == CategoryGroupID
                                 select category).ToList();

            return Json(new SelectList(categoryTypesList, "Id", "Name"));
        }
    }
}