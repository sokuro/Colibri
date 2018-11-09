using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Colibri.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Colibri.Utility;

namespace Colibri.Areas.Identity.Pages.Account
{
    //[Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        // Input Model used for Razor Page
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // extended Properties
            [Required]
            public string UserName { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Super Admin")]
            public bool IsSuperAdmin { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        // POST Handler
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                // extend the Properties
                var user = new ApplicationUser {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber};

                // create a new User inside the DB
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
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
                    }

                    if (Input.IsSuperAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.SuperAdminEndUser);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.AdminEndUser);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    // Sending Confirmation EMail
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // prevent newly registered Users from being automatically logged on
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // redirect to the List of Users
                    // TODO
                    return RedirectToAction("Index", "AdminUsers", new { area = "Admin" });
                    //return RedirectToAction("Index", "AdminUsers");
                    //return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
