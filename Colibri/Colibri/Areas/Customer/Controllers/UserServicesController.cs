using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
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
     * Controller for the User Services View
     *
     * authorize only the AdminEndUser (registered User)
     */
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class UserServicesController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<UserServicesController> _localizer;

        // bind to the UserServices ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public UserServicesViewModel UserServicesViewModel { get; set; }

        public UserServicesController(ColibriDbContext colibriDbContext, 
            HostingEnvironment hostingEnvironment,
            IStringLocalizer<UserServicesController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _hostingEnvironment = hostingEnvironment;
            _localizer = localizer;

            // initialize the Constructor for the ProductsController
            UserServicesViewModel = new UserServicesViewModel()
            {
                CategoryGroups = _colibriDbContext.CategoryGroups.ToList(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList(),
                SpecialTags = _colibriDbContext.SpecialTags.ToList(),
                UserServices = new List<UserServices>(),
                Users = new List<ApplicationUser>()
            };
        }

        // Index
        [Route("Customer/UserServices/Index")]
        public async Task<IActionResult> Index(
            string searchUserName = null,
            string searchServiceName = null)
        {
            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/UserService?servicePage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchName=");
            if (searchServiceName != null)
            {
                param.Append(searchServiceName);
            }

            // populate the Lists
            UserServicesViewModel.UserServices = await _colibriDbContext.UserServices.ToListAsync();
            UserServicesViewModel.Users = from u in _colibriDbContext.ApplicationUsers
                                                    join p in _colibriDbContext.Products
                                                    .Include(p => p.ApplicationUser)
                                                    .ThenInclude(p => p.UserName)
                                                    on u.Id equals p.ApplicationUserId
                                                    select u;

            // Search Conditions
            if (searchUserName != null)
            {
                UserServicesViewModel.UserServices = UserServicesViewModel.UserServices
                    .Where(a => a.ApplicationUser.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchServiceName != null)
            {
                UserServicesViewModel.UserServices = UserServicesViewModel.UserServices
                    .Where(a => a.Name.ToLower().Contains(searchServiceName.ToLower())).ToList();
            }

            // i18n
            ViewData["UserService"] = _localizer["UserServiceText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["NewUserService"] = _localizer["NewUserServiceText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["NumberOfClicks"] = _localizer["NumberOfClicksText"];

            return View(UserServicesViewModel);
        }
    }
}