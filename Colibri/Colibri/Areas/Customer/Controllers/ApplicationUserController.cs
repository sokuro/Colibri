using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller to manage Application Users
     * 
     * authorize for only for the registered Users
     */
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    [Area("Customer")]
    public class ApplicationUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}