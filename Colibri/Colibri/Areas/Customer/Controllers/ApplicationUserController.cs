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

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller to manage Application Users
     * 
     * authorize for only for the registered Users (and SuperAdmin)
     */
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class ApplicationUserController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // CTOR
        // get the Data from the DB
        public ApplicationUserController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        public IActionResult Index()
        {
            // Application User ViewModel
            ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel
            {
                // initialize
                ApplicationUsers = new List<Models.ApplicationUser>()
            };

            // populate the List
            applicationUserViewModel.ApplicationUsers = _colibriDbContext.ApplicationUsers.ToList();

            // return the List of registered Application Users
            return View(applicationUserViewModel);
        }
    }
}