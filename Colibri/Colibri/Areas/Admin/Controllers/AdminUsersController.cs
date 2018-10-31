using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Admin.Controllers
{
    /*
     * Controller to manage Application Users outside the DB
     */
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        public AdminUsersController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }
        public IActionResult Index()
        {
            // return the List of the Application Users
            var listOfAppUsers = _colibriDbContext.ApplicationUsers.ToList();

            return View(listOfAppUsers);
        }
    }
}