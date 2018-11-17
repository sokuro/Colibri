using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller for the Advertisement View
     * 
     * authorize only the AdminEndUser (registered User)
     */
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class AdvertisementController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // bind to the ViewModel
        // not necessary to create new Objects
        [BindProperty]
        public AdvertisementViewModel AdvertisementViewModel { get; set; }

        public AdvertisementController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;

            // initialize the Constructor for the AdvertisementController
            AdvertisementViewModel = new AdvertisementViewModel()
            {
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                Products = new Models.Products()
            };
        }

        // Index
        public async Task<IActionResult> Index()
        {
            var productList = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .ToListAsync();

            return View(productList);
        }

        // create a new Advertisement
        // pass the ViewModel for the DropDown Functionality of the Category Types
        public IActionResult Create()
        {
            return View(AdvertisementViewModel);
        }
    }
}