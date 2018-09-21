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
        // Phone
        [Route("")]
        public string Phone()
        {
            return "031 301 03 47";
        }

        // Address
        [Route("address")]
        public string Address()
        {
            return "CH";
        }
    }
}