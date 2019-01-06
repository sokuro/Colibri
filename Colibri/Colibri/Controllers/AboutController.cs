using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Colibri
{
    [Route("about")]
    public class AboutController : Controller
    {
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        // Phone
        [Route("")]
        public string Phone()
        {
            return "123";
        }

        // Address
        [Route("address")]
        public string Address()
        {
            return "CH";
        }
    }
}