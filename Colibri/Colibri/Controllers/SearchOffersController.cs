using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Customer.Controllers
{
    /*
* Controller for the Advertisement View
* 
* authorize only the AdminEndUser (registered User)
*/
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    //[Area("Customer")]
    public class SearchOffersController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SearchOffersController> _localizer;

        // bind to the Search-ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public SearchViewModel SearchViewModel { get; set; }

        // Constructor
        public SearchOffersController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<SearchOffersController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // Search ViewModel
            SearchViewModel = new SearchViewModel()
            {
                Products = new Models.Products(),
                UserServices = new Models.UserServices()
            };

        }

        // GET : Action for Index
        //[Route("SearchOffers/Index")]
        public async Task<IActionResult> Index()
        {
            // i18n
            ViewData["SearchOffers"] = _localizer["SearchOffersText"];
            ViewData["SearchString"] = _localizer["SearchStringText"];
            ViewData["Details"] = _localizer["DetailsText"];
            ViewData["Sorry1"] = _localizer["Sorry1Text"];
            ViewData["Sorry2"] = _localizer["Sorry2Text"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["Products"] = _localizer["ProductsText"];
            ViewData["Service"] = _localizer["ServiceText"];
            ViewData["Search"] = _localizer["SearchText"];

            // Products
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Where(m => m.isOffer == true).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();

            // Userservices
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Where(m => m.isOffer == true).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            SearchViewModel.CategoryGroupsList = await _colibriDbContext.CategoryGroups.ToListAsync();

            //// Filter auf Angebote
            //SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.isOffer == true);
            //SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.isOffer == true);

            return View(SearchViewModel);
        }

        // POST : Action for Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // i18n
            ViewData["SearchOffers"] = _localizer["SearchOffersText"];
            ViewData["SearchString"] = _localizer["SearchStringText"];
            ViewData["Details"] = _localizer["DetailsText"];
            ViewData["Sorry1"] = _localizer["Sorry1Text"];
            ViewData["Sorry2"] = _localizer["Sorry2Text"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["PLZ"] = _localizer["PLZText"];
            ViewData["Products"] = _localizer["ProductsText"];
            ViewData["Service"] = _localizer["ServiceText"];
            ViewData["Search"] = _localizer["SearchText"];

            // ProductsList and UserServiceList
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Where(m => m.isOffer == true).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Where(m => m.isOffer == true).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();

            // check if modelstate is valid
            // if modelstate is not valid, return to Index
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // Prüfen, ob Suchbegriff für Inserat existiert
            if (!string.IsNullOrEmpty(model.SearchAdvertisement))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.Name.ToLower().Contains(model.SearchAdvertisement.ToLower()));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.Name.ToLower().Contains(model.SearchAdvertisement.ToLower()));
            }

            // Prüfen, ob Suchbegriff für Rubrik-Gruppe existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryGroup))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.CategoryGroups.Name.ToLower().Contains(model.SearchCategoryGroup.ToLower()));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.CategoryGroups.Name.ToLower().Contains(model.SearchCategoryGroup.ToLower()));
            }

            // Prüfen, ob Suchbegriff für Rubrik existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryType))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.CategoryTypes.Name.ToLower().Contains(model.SearchCategoryType.ToLower()));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.CategoryTypes.Name.ToLower().Contains(model.SearchCategoryType.ToLower()));
            }


            // Prüfen, ob Suchbegriff für PLZ existiert
            if (!string.IsNullOrEmpty(model.PLZ))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.CategoryTypes.PLZ.Contains(model.PLZ));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.CategoryTypes.PLZ.Contains(model.PLZ));
            }

            // Filter auf Angebote
            SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.isOffer == true);
            SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.isOffer == true);

            // Counter updaten
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            // Return View
            return View(SearchViewModel);
        }

        // GET : Action for SearchOffer
        public async Task<IActionResult> SearchOffer(HomeIndexViewModel model)
        {
            // i18n
            ViewData["SearchOffers"] = _localizer["SearchOffersText"];
            ViewData["SearchString"] = _localizer["SearchStringText"];
            ViewData["Details"] = _localizer["DetailsText"];
            ViewData["Sorry1"] = _localizer["Sorry1Text"];
            ViewData["Sorry2"] = _localizer["Sorry2Text"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["Products"] = _localizer["ProductsText"];
            ViewData["Service"] = _localizer["ServiceText"];

            SearchViewModel.SearchAdvertisement = model.SearchAdvertisement;

            // Prüfen, ob es ein aktuelles PRODUKTE-Angebot in der Datenbank gibt
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == true).ToListAsync();
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();

            // Prüfen, ob es ein aktuelles DIENSTLEISTUNGS-Angebot in der Datenbank gibt
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == true).ToListAsync();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            // Gesamte Anzahl Resultate
            SearchViewModel.ResultsCounter = SearchViewModel.ProductsCounter + SearchViewModel.UserServicesCounter;

            // Falls kein aktuelles Angebot in der Datenbank gefunden wird, wird im Archiv gesucht, ob in der Vergangenheit ein passendes Angebot erfasst wurde
            if (SearchViewModel.ResultsCounter < 1)
            {
                // Ergebnisse werden absteigend sortiert und die Top 3 Werte werden zurückgegeben
                SearchViewModel.ArchiveEntryList = await _colibriDbContext.ArchiveEntry.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == true).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).OrderByDescending(p => p.CreatedOn).Take(3).ToListAsync();
                SearchViewModel.ResultsCounterArchive = SearchViewModel.ArchiveEntryList.Count();
            }

            // Einträge für SUCHANFRAGEN in Tabelle SearchEntry schreiben
            if(SearchViewModel.SearchAdvertisement != null)
            {
                if (SearchViewModel.SearchAdvertisement.Length > 2)
                {
                    var userSearch = new SearchEntry();

                    userSearch.SearchDate = System.DateTime.Now;
                    userSearch.SearchText = SearchViewModel.SearchAdvertisement;
                    userSearch.Counter = 1;
                    userSearch.SearchOffer = true;

                    // Falls Resultat vorhanden
                    if (SearchViewModel.ResultsCounter > 0)
                    {
                        userSearch.FullSuccess = true;
                        userSearch.PartSuccess = false;
                        userSearch.NoSuccess = false;
                    }

                    // Falls Resultat in Archive gefunden wird
                    else
                    {
                        if (SearchViewModel.ResultsCounterArchive > 0)
                        {
                            userSearch.FullSuccess = false;
                            userSearch.PartSuccess = true;
                            userSearch.NoSuccess = false;
                        }
                        // Falls kein Resultat gefunden wird
                        else
                        {
                            userSearch.FullSuccess = false;
                            userSearch.PartSuccess = false;
                            userSearch.NoSuccess = true;
                        }
                    }

                    // Add userSearch to DB and save changes
                    _colibriDbContext.SearchEntry.Add(userSearch);
                    await _colibriDbContext.SaveChangesAsync();
                }
            }
            return View(SearchViewModel);
        }

        // Get Category 
        [Route("SearchOffers/GetCategory")]
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