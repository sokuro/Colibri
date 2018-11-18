﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DashboardMiddleLayerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}