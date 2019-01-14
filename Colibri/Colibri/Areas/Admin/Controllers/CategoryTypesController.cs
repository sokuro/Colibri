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

        // bind to the Search-ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public SearchViewModel SearchViewModel { get; set; }

        public CategoryTypesController(ColibriDbContext colibriDbContext, IStringLocalizer<CategoryTypesController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            // Search ViewModel
            SearchViewModel = new SearchViewModel()
            {
                Products = new Models.Products(),
                UserServices = new Models.UserServices(),
                PLZ = new string("")
            };
        }

        // GET : Action Index
        [Route("Admin/CategoryTypes/Index")]
        public async Task<IActionResult> Index()
        {
            SearchViewModel.CategoryTypesList = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).ToListAsync();

            //var categoryTypesList = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).ToListAsync();

            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CategoryType1"] = _localizer["CategoryType1Text"];
            ViewData["NewCategoryType"] = _localizer["NewCategoryTypeText"];
            ViewData["NewCategoryTypeUserService"] = _localizer["NewCategoryTypeUserServiceText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Overview"] = _localizer["OverviewText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["ProductService"] = _localizer["ProductServiceText"];

            // Update Resultscounter
            SearchViewModel.ResultsCounter = SearchViewModel.CategoryTypesList.Count();

            return View(SearchViewModel);
        }

        [Route("Admin/CategoryTypes/Index")]
        // POST : Action for Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CategoryType1"] = _localizer["CategoryType1Text"];
            ViewData["NewCategoryType"] = _localizer["NewCategoryTypeText"];
            ViewData["NewCategoryTypeUserService"] = _localizer["NewCategoryTypeUserServiceText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Overview"] = _localizer["OverviewText"];
            ViewData["ProductService"] = _localizer["ProductServiceText"];

            // CategoryTypesList
            SearchViewModel.CategoryTypesList = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).ToListAsync();

            // ResultsCounter initialisieren
            SearchViewModel.ResultsCounter = 0;
            SearchViewModel.PLZ = "";

            // check if modelstate is valid
            // if modelstate is not valid, return to Index
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // Prüfen, ob Suchbegriff für Rubrik-Gruppe existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryGroup))
            {
                SearchViewModel.CategoryTypesList = SearchViewModel.CategoryTypesList.Where(m => m.CategoryGroups.Name.Contains(model.SearchCategoryGroup));
            }

            // Prüfen, ob Suchbegriff für Rubrik existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryType))
            {
                SearchViewModel.CategoryTypesList = SearchViewModel.CategoryTypesList.Where(m => m.Name.Contains(model.SearchCategoryType));
            }

            // Prüfen, ob Suchbegriff für PLZ existiert
            if (!string.IsNullOrEmpty(model.PLZ))
            {
                SearchViewModel.CategoryTypesList = SearchViewModel.CategoryTypesList.Where(m => m.PLZ != null);
                SearchViewModel.CategoryTypesList = SearchViewModel.CategoryTypesList.Where(m => m.PLZ.Contains(model.PLZ));
            }

            // ResultsCounter aktualisieren
            SearchViewModel.ResultsCounter = SearchViewModel.CategoryTypesList.Count();

            // Return View
            return View(SearchViewModel);
        }

        // Get: /<controller>/Create
        [Route("Admin/CategoryTypes/Create")]
        public IActionResult Create()
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["IsNew"] = _localizer["IsNewText"];
            ViewData["ExistingCategories"] = _localizer["ExistingCategoriesText"];


            CategoryTypesAndCategoryGroupsViewModel model = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.Where(m => m.TypeOfCategoryGroup.Equals("Product")).ToList(),
                CategoryTypes = new CategoryTypes(),
                CategoryTypesList = _colibriDbContext.CategoryTypes.Where(m => m.CategoryGroups.TypeOfCategoryGroup.Equals("Product")).OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
            };

            return View(model);
        }

        // Get: /<controller>/CreateUserService
        [Route("Admin/CategoryTypes/CreateUserService")]
        public IActionResult CreateUserService()
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["CreateCategoryServiceType"] = _localizer["CreateCategoryServiceTypeText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["IsNew"] = _localizer["IsNewText"];
            ViewData["ExistingCategories"] = _localizer["ExistingCategoriesText"];


            CategoryTypesAndCategoryGroupsViewModel model = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.Where(m => m.TypeOfCategoryGroup.Equals("Service")).ToList(),
                CategoryTypes = new CategoryTypes(),
                CategoryTypesList = _colibriDbContext.CategoryTypes.Where(m => m.CategoryGroups.TypeOfCategoryGroup.Equals("Service")).OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
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
                    if (doesCategoryTypesExist == 0 && !model.isNew)
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
                            if(model.CategoryTypes.PLZ == null)
                            {
                                model.CategoryTypes.isGlobal = true;
                            }

                            // Wenn keine Fehler, kombinierten Name ergänzen
                            // Product / UserService
                            model.CategoryTypes.CategoryGroups = await _colibriDbContext.CategoryGroups.Where(m => m.Id == model.CategoryTypes.CategoryGroupId).FirstOrDefaultAsync();

                            if(model.CategoryTypes.CategoryGroups.TypeOfCategoryGroup.Equals("Product"))
                            {
                                model.CategoryTypes.NameCombined = "Product - " + model.CategoryTypes.CategoryGroups.Name + " - " + model.CategoryTypes.Name;
                            }
                            else
                            {
                                model.CategoryTypes.NameCombined = "Service - " + model.CategoryTypes.CategoryGroups.Name + " - " + model.CategoryTypes.Name;
                            }
                            
                            // Eintrag in DB schreiben
                            _colibriDbContext.Add(model.CategoryTypes);
                            await _colibriDbContext.SaveChangesAsync();
                            

                            // Publish the Created Category Type
                            //using (var bus = RabbitHutch.CreateBus("host=localhost"))
                            //{
                            //    //bus.Publish(categoryTypes, "create_category_types");
                            //    await bus.SendAsync("create_category_types", model.CategoryTypes);
                            //}

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

        // Post: /<controller>/Create
        // @param Category
        [Route("Admin/CategoryTypes/CreateUserService")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserService(CategoryTypesAndCategoryGroupsViewModel model)
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["CreateCategoryServiceType"] = _localizer["CreateCategoryServiceTypeText"];
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
                    if (doesCategoryTypesExist == 0 && !model.isNew)
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
                            if (model.CategoryTypes.PLZ == null)
                            {
                                model.CategoryTypes.isGlobal = true;
                            }

                            // Wenn keine Fehler, kombinierten Name ergänzen
                            // Product / UserService
                            model.CategoryTypes.CategoryGroups = await _colibriDbContext.CategoryGroups.Where(m => m.Id == model.CategoryTypes.CategoryGroupId).FirstOrDefaultAsync();

                            if (model.CategoryTypes.CategoryGroups.TypeOfCategoryGroup.Equals("Product"))
                            {
                                model.CategoryTypes.NameCombined = "Product - " + model.CategoryTypes.CategoryGroups.Name + " - " + model.CategoryTypes.Name;
                            }
                            else
                            {
                                model.CategoryTypes.NameCombined = "Service - " + model.CategoryTypes.CategoryGroups.Name + " - " + model.CategoryTypes.Name;
                            }

                            // Eintrag in DB schreiben
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

            // Viewmodel
            CategoryTypesAndCategoryGroupsViewModel model = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = categoryType,
                CategoryTypesList = _colibriDbContext.CategoryTypes.Select(p => p.Name).Distinct().ToList()
            };

            // i18n
            ViewData["EditCategoryType"] = _localizer["EditCategoryTypeText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["IsNew"] = _localizer["IsNewText"];
            ViewData["ExistingCategories"] = _localizer["ExistingCategoriesText"];

            return View(model);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Admin/CategoryTypes/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryTypesAndCategoryGroupsViewModel model)
        {
            // i18n
            ViewData["CreateCategoryType"] = _localizer["CreateCategoryTypeText"];
            ViewData["Create"] = _localizer["CreateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["Name"] = _localizer["NameText"];

            // check if CategoryTypes exists or not & check if Combination of CategoryTypes and CategoryGroup exists
            var doesCategoryTypesExist = _colibriDbContext.CategoryTypes.Where(s => s.Name == model.CategoryTypes.Name).Count();
            var doesCategoryTypesAndCategoryGroupsExist = _colibriDbContext.CategoryTypes.Where(s => s.Name == model.CategoryTypes.Name && s.CategoryGroupId == model.CategoryTypes.CategoryGroupId).Count();

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                if(doesCategoryTypesExist == 0)
                {
                    // error
                    StatusMessage = "Error : CategoryTypes does not exist. You cannot add a new CategoryTypes here.";
                }
                else
                {
                    if(doesCategoryTypesAndCategoryGroupsExist > 0)
                    {
                        // error
                        StatusMessage = "Error : CategoryTypes and CategoryGroups combination already exists!";
                    }
                    else
                    {
                        if (model.CategoryTypes.PLZ == null)
                        {
                            model.CategoryTypes.isGlobal = true;
                        }
                        else
                        {
                            model.CategoryTypes.isGlobal = false;
                        }

                        // Wenn keine Fehler, Eintrag in DB hinzufügen
                        var catTypeFromDb = _colibriDbContext.CategoryTypes.Find(id);
                        catTypeFromDb.Name = model.CategoryTypes.Name;
                        catTypeFromDb.CategoryGroupId = model.CategoryTypes.CategoryGroupId;

                        await _colibriDbContext.SaveChangesAsync();

                        // Publish the Created Category Type
                        //using (var bus = RabbitHutch.CreateBus("host=localhost"))
                        //{
                        //    //bus.Publish(categoryTypes, "create_category_types");
                        //    await bus.SendAsync("create_category_types", model.CategoryTypes);
                        //}

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            // If ModelState is not valid
            CategoryTypesAndCategoryGroupsViewModel modelVM = new CategoryTypesAndCategoryGroupsViewModel()
            {
                CategoryGroupsList = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = model.CategoryTypes,
                CategoryTypesList = _colibriDbContext.CategoryTypes.Select(p => p.Name).Distinct().ToList(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
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
            ViewData["PLZ"] = _localizer["PLZText"];

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
            var categoryType = await _colibriDbContext.CategoryTypes.Include(s => s.CategoryGroups).SingleOrDefaultAsync(m => m.Id == id);

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