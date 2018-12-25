using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Areas.Customer.Controllers;
using Colibri.Data;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Controllers
{
    /*
* Controller for the Advertisement View
* 
* authorize only the AdminEndUser (registered User)
*/
    public class SearchRequestsController : Controller
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
        public SearchRequestsController(ColibriDbContext colibriDbContext,
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
        public async Task<IActionResult> Index()
        {
            // Products
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();

            // Userservices
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            SearchViewModel.CategoryGroupsList = await _colibriDbContext.CategoryGroups.ToListAsync();

            // Filter auf Nachfragen
            SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.isOffer == false);
            SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.isOffer == false);

            return View(SearchViewModel);
        }

        // POST : Action for Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // ProductsList and UserServiceList
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();

            // check if modelstate is valid
            // if modelstate is not valid, return to Index
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // Prüfen, ob Suchbegriff für Inserat existiert
            if (!string.IsNullOrEmpty(model.SearchAdvertisement))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.Name.Contains(model.SearchAdvertisement));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.Name.Contains(model.SearchAdvertisement));
            }

            // Prüfen, ob Suchbegriff für Rubrik-Gruppe existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryGroup))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.CategoryGroups.Name.Contains(model.SearchCategoryGroup));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.CategoryGroups.Name.Contains(model.SearchCategoryGroup));
            }

            // Prüfen, ob Suchbegriff für Rubrik existiert
            if (!string.IsNullOrEmpty(model.SearchCategoryType))
            {
                SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.CategoryTypes.Name.Contains(model.SearchCategoryType));
                SearchViewModel.UserServicesList = SearchViewModel.UserServicesList.Where(m => m.CategoryTypes.Name.Contains(model.SearchCategoryType));
            }

            // Counter updaten
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            // Return View
            return View(SearchViewModel);
        }

        // GET : Action for SearchRequest
        public async Task<IActionResult> SearchRequest(HomeIndexViewModel model)
        {
            // i18n
            ViewData["SearchRequests"] = _localizer["SearchRequestsText"];
            ViewData["SearchString"] = _localizer["SearchStringText"];
            ViewData["Details"] = _localizer["DetailsText"];
            ViewData["Sorry1"] = _localizer["Sorry1Text"];
            ViewData["Sorry2"] = _localizer["Sorry2Text"];
            ViewData["Title"] = _localizer["TitleText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];

            SearchViewModel.SearchAdvertisement = model.SearchAdvertisement;

            // Prüfen, ob es ein aktuelles PRODUKTE-Angebot in der Datenbank gibt
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == false).ToListAsync();
            SearchViewModel.ProductsCounter = SearchViewModel.ProductsList.Count();

            // Prüfen, ob es ein aktuelles DIENSTLEISTUNGS-Angebot in der Datenbank gibt
            SearchViewModel.UserServicesList = await _colibriDbContext.UserServices.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == false).ToListAsync();
            SearchViewModel.UserServicesCounter = SearchViewModel.UserServicesList.Count();

            // Gesamte Anzahl Resultate
            SearchViewModel.ResultsCounter = SearchViewModel.ProductsCounter + SearchViewModel.UserServicesCounter;

            // Falls kein aktuelles Angebot in der Datenbank gefunden wird, wird im Archiv gesucht, ob in der Vergangenheit ein passendes Angebot erfasst wurde
            if (SearchViewModel.ResultsCounter < 1)
            {
                // Ergebnisse werden absteigend sortiert und die Top 3 Werte werden zurückgegeben
                SearchViewModel.ArchiveEntryList = await _colibriDbContext.ArchiveEntry.Where(m => m.Name.Contains(SearchViewModel.SearchAdvertisement)).Where(m => m.isOffer == false).Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).OrderByDescending(p => p.CreatedOn).Take(3).ToListAsync();
                SearchViewModel.ResultsCounterArchive = SearchViewModel.ArchiveEntryList.Count();
            }

            return View(SearchViewModel);
        }

    }
}