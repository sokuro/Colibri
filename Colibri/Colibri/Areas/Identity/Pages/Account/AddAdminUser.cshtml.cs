using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Models;
using Colibri.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Colibri.Areas.Identity.Pages.Account
{
    /*
     * PageModel for the AddAdminUser Page
     */
    public class AddAdminUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddAdminUserModel(
            UserManager<IdentityUser> userManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGet()
        {
            // create Roles and SuperAdminUser
            // first check if the Role exists, if not -> create
            // #1: Admin
            if (!await _roleManager.RoleExistsAsync(StaticDetails.AdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser));
            }

            // #1: Super Admin
            if (!await _roleManager.RoleExistsAsync(StaticDetails.SuperAdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser));
                // hardcoded AdminUser
                var userAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@yacrol.com",
                    PhoneNumber = "0313010347",
                    FirstName = "Site",
                    LastName = "Admin"
                };

                // create the AdminUser
                // UserName + Password
                var resultUser = await _userManager.CreateAsync(userAdmin, "Admin123#");
                await _userManager.AddToRoleAsync(userAdmin, StaticDetails.SuperAdminEndUser);
            }

            return Page();
        }
    }
}