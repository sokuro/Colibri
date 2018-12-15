using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IStringLocalizer<AdvertisementController> _localizer;

        // bind to the Search-ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public SearchViewModel SearchViewModel { get; set; }

        // Constructor
        public SearchOffersController(ColibriDbContext colibriDbContext,
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<AdvertisementController> localizer)
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
            SearchViewModel.ProductsList = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();
            SearchViewModel.CategoryGroupsList = _colibriDbContext.CategoryGroups.ToList();
            SearchViewModel.CategoryTypesList = _colibriDbContext.CategoryTypes.ToList();

            // Filter auf Angebote
            SearchViewModel.ProductsList = SearchViewModel.ProductsList.Where(m => m.isOffer = true);

            return View(SearchViewModel);
        }

        // POST : Action for Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // check if modelstate is valid
            // if modelstate is not valid, return to Index
                if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // if modelstate is valid
            var listOffersProducts = await _colibriDbContext.Products.Include(m => m.CategoryGroups).Include(m => m.CategoryTypes).ToListAsync();

            // Dummy View
            return View();
        }
    }
}