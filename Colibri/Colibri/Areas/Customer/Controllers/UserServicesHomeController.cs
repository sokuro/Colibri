using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Areas.Admin.Controllers;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller for the User Services View
     */
    [Area("Customer")]
    public class UserServicesHomeController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<UserServicesHomeController> _localizer;

        public UserServicesHomeController(ColibriDbContext colibriDbContext, IStringLocalizer<UserServicesHomeController> localizer)
        {
            //_repository = repository;
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;
        }

        // Entry (Index) View
        [Route("Customer/UserServicesHome/Index")]
        public async Task<IActionResult> Index()
        {
            var userServicesList = await _colibriDbContext.UserServices
                    .Include(p => p.CategoryGroups)
                    .Include(p => p.CategoryTypes)
                    .ToListAsync();

            // i18n
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];

            return View(userServicesList);
        }

        // Details View GET
        // authorize only the AdminEndUser(registered User)
        [Route("Customer/UserServicesHome/Details/{id}")]
        [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
        public async Task<IActionResult> Details(int id)
        {
            // get the individual Product
            var userService = await _colibriDbContext.UserServices
                    .Include(p => p.CategoryGroups)
                    .Include(p => p.CategoryTypes)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            // count the Number of Clicks on the Product
            userService.NumberOfClicks += 1;

            // save the Changes in DB
            await _colibriDbContext.SaveChangesAsync();

            // i18n
            ViewData["UserServiceDetails"] = _localizer["UserServiceDetailsText"];
            ViewData["Name"] = _localizer["NameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["SpecialTag"] = _localizer["SpecialTagText"];
            ViewData["RemoveFromBag"] = _localizer["RemoveFromBagText"];
            ViewData["Order"] = _localizer["OrderText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(userService);
        }

        // Details POST
        [Route("Customer/UserServicesHome/Details/{id}")]
        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check first, if anything exists in the Session
            // Session Name : "ssScheduling"
            List<int> lstCartServices = HttpContext.Session.Get<List<int>>("userServicesScheduling");

            // check if null -> create new
            if (lstCartServices == null)
            {
                lstCartServices = new List<int>();
            }

            // add the retrieved Item (id)
            lstCartServices.Add(id);
            // set the Session:
            // Session Name, Value
            HttpContext.Session.Set("userServicesScheduling", lstCartServices);

            // redirect to Action
            return RedirectToAction("Index", "Scheduling", new { area = "Customer" });
        }

        // Remove (from Bag)
        [Route("Customer/UserServicesHome/Remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<int> lstCartServices = HttpContext.Session.Get<List<int>>("userServicesScheduling");

            if (lstCartServices != null && lstCartServices.Any())
            {
                if (lstCartServices.Contains(id))
                {
                    // remove the Item (id)
                    lstCartServices.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("userServicesScheduling", lstCartServices);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }

    }
}