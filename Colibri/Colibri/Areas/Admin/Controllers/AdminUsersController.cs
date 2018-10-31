using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Areas.Admin.Controllers
{
    /*
     * Controller to manage Application Users outside the DB
     */
     // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        public AdminUsersController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        // Main Index EntryPage
        public IActionResult Index()
        {
            // return the List of the Application Users
            var listOfAppUsers = _colibriDbContext.ApplicationUsers.ToList();

            return View(listOfAppUsers);
        }

        // Get: Method Edit User
        // ID -> GUI (as string)
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

            // pass the User to the View
            return View(userFromDb);
        }

        // POST: Method Edit User
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
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;

                // save Changes
                _colibriDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);
        }

        // Get: Method Delete User
        // ID -> GUI (as string)
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

            // pass the User to the View
            return View(userFromDb);
        }

        // POST: Method Delete User
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