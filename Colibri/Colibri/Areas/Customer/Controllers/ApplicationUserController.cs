using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly IEmailSender _emailSender;

        // CTOR
        // get the Data from the DB
        public ApplicationUserController(ColibriDbContext colibriDbContext, IEmailSender emailSender)
        {
            _colibriDbContext = colibriDbContext;
            _emailSender = emailSender;
        }

        // extend the Method with the Parameters for Search:
        // UserName, FirstName, LastName
        public async Task<IActionResult> Index(
            string searchUserName=null,
            string searchFirstName=null,
            string searchLastName=null
            )
        {
            // Application User ViewModel
            ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel
            {
                // initialize
                ApplicationUsers = new List<Models.ApplicationUser>()
            };

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchEmail=");
            if (searchFirstName != null)
            {
                param.Append(searchFirstName);
            }
            param.Append("&searchPhone=");
            if (searchLastName != null)
            {
                param.Append(searchLastName);
            }

            // populate the List
            applicationUserViewModel.ApplicationUsers = _colibriDbContext.ApplicationUsers.ToList();

            // Search Conditions
            if (searchUserName != null)
            {
                applicationUserViewModel.ApplicationUsers = applicationUserViewModel.ApplicationUsers.Where(a => a.UserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchFirstName != null)
            {
                applicationUserViewModel.ApplicationUsers = applicationUserViewModel.ApplicationUsers.Where(a => a.FirstName.ToLower().Contains(searchFirstName.ToLower())).ToList();
            }
            if (searchLastName != null)
            {
                applicationUserViewModel.ApplicationUsers = applicationUserViewModel.ApplicationUsers.Where(a => a.LastName.ToLower().Contains(searchLastName.ToLower())).ToList();
            }

            // return the List of registered Application Users
            return View(applicationUserViewModel);
        }

        // Method Details GET
        public async Task<IActionResult> Details(string id)
        {
            // get the individual User
            var user = await _colibriDbContext.ApplicationUsers
                            .Where(u => u.Id == id)
                            .FirstOrDefaultAsync();

            return View(user);
        }

        // Send Mail by Contact
        // GET: /<controller>/Contact
        [HttpGet("contactUser")]
        public ActionResult ContactUser()
        {
            return View();
        }

        // POST: /<controller>/Contact
        [HttpPost("contactUser")]
        public async Task<ActionResult> ContactUserAsync(ContactViewModel model, string id)
        {
            if (ModelState.IsValid)
            {
                // first get the individual User to send the Mail to
                var user = await _colibriDbContext.ApplicationUsers
                            .Where(u => u.Id == id)
                            .FirstOrDefaultAsync();

                // Send the notification email
                await _emailSender.SendEmailAsync(user.Email, model.Subject,
                    $"User {model.Name} with the Email Address {model.Email} sent you this Message: <br/> {model.Message}");

                // display Sent Message
                ViewBag.UserMessage = "Message sent";
                // clear the Model
                ModelState.Clear();
            }
            return RedirectToAction("Index", "ApplicationUser", new { area = "Customer" });
        }
    }
}