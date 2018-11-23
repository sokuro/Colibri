using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DashboardMiddleLayerController : Controller
    {
        [Route("Customer/DashboardMiddleLayer/Index")]

        public IActionResult Index()
        {
            return View();
        }
    }
}