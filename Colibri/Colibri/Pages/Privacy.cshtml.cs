using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Colibri.Pages
{
    public class PrivacyModel : PageModel
    {
        public string ReturnUrl { get; set; }

        [Route("/Pages/Privacy")]
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
    }
}