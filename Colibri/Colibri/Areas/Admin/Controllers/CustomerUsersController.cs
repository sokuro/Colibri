using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Areas.Customer.Controllers;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    /*
     * Controller to manage Application Users outside the DB
     */
     // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class CustomerUsersController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<CustomerUsersController> _localizer;

        public CustomerUsersController(ColibriDbContext colibriDbContext, IStringLocalizer<CustomerUsersController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;
        }

        // Main Index EntryPage
        [Route("Admin/CustomerUsers/Index")]
        public async Task<IActionResult> Index()
        {
            // return the List of the Super Admin Users
            var listOfCustomerUsers = await _colibriDbContext.ApplicationUsers
                                    .Where(u => u.IsSuperAdmin == false)
                                    .ToListAsync();

            // i18n
            ViewData["CustomerUser"] = _localizer["CustomerUserText"];
            ViewData["NewCustomerUser"] = _localizer["NewCustomerUserText"];
            ViewData["FirstName"] = _localizer["FirstNameText"];
            ViewData["LastName"] = _localizer["LastNameText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberText"];
            ViewData["Disabled"] = _localizer["DisabledText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Delete"] = _localizer["DeleteText"];

            return View(listOfCustomerUsers);
        }

        // Get: Method Edit User
        // ID -> GUI (as string)
        [Route("Admin/CustomerUsers/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            // retrieve the User from the DB
            var userFromDb = await _colibriDbContext.ApplicationUsers.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditCustomerUser"] = _localizer["EditCustomerUserText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["FirstName"] = _localizer["FirstNameText"];
            ViewData["LastName"] = _localizer["LastNameText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberText"];
            ViewData["Street"] = _localizer["StreetText"];
            ViewData["CareOf"] = _localizer["CareOfText"];
            ViewData["City"] = _localizer["CityText"];
            ViewData["Zip"] = _localizer["ZipText"];
            ViewData["Country"] = _localizer["CountryText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            // pass the User to the View
            return View(userFromDb);
        }

        // POST: Method Edit User
        [Route("Admin/CustomerUsers/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _colibriDbContext.ApplicationUsers
                                                    .Where(u => u.Id == id)
                                                    .FirstOrDefault();
                // Properties or the User
                userFromDb.FirstName = applicationUser.FirstName;
                userFromDb.LastName = applicationUser.LastName;
                // TODO: User can edit Email?!
                userFromDb.Email = applicationUser.Email;
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;
                userFromDb.Street = applicationUser.Street;
                userFromDb.CareOf = applicationUser.CareOf;
                userFromDb.City = applicationUser.City;
                userFromDb.Zip = applicationUser.Zip;
                userFromDb.Country = applicationUser.Country;
                userFromDb.Modified = DateTime.Now;

                // save Changes
                _colibriDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);
        }

        // Get: Method Delete User
        // ID -> GUI (as string)
        [Route("Admin/CustomerUsers/Delete/{id}")]
        public async Task<IActionResult> Delete (string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            // retrieve the User from the DB
            var userFromDb = await _colibriDbContext.ApplicationUsers.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteCustomerUser"] = _localizer["DeleteCustomerUserText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["FirstName"] = _localizer["FirstNameText"];
            ViewData["LastName"] = _localizer["LastNameText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            // pass the User to the View
            return View(userFromDb);
        }

        // POST: Method Delete User
        [Route("Admin/CustomerUsers/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost (string id)
        {
            ApplicationUser userFromDb = _colibriDbContext.ApplicationUsers
                                                .Where(u => u.Id == id)
                                                .FirstOrDefault();
            // set the Lockout for the User with specific Time
            // @param years = 100y
            userFromDb.LockoutEnd = DateTime.Now.AddYears(100);

            // save Changes
            _colibriDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}