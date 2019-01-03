using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller to Handle Application User's Ratings
     */
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class ApplicationUserRatingController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<ApplicationUserRatingController> _localizer;

        // PageSize (for the Pagination: 10 User Ratings/Page)
        private int PageSize = 10;

        [BindProperty]
        public ApplicationUserRatingViewModel ApplicationUserRatingViewModel { get; set; }

        public ApplicationUserRatingController(ColibriDbContext colibriDbContext,
            IStringLocalizer<ApplicationUserRatingController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            ApplicationUserRatingViewModel = new ApplicationUserRatingViewModel()
            {
                ApplicationUsers = new List<ApplicationUserRatings>(),
                ApplicationUser = new Models.ApplicationUserRatings()
            };
        }

        [Route("Customer/ApplicationUserRating/Index")]
        public IActionResult Index(
            int applicationUserPage = 1,
            string searchUserName = null,
            int searchUserRating = 0
            )
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUserRatingViewModel.CurrentUserId = claim.Value;

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/ApplicationUserRating/Index?applicationUserPage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            if (searchUserRating != 0)
            {
                param.Append(searchUserRating);
            }

            // populate the Lists
            ApplicationUserRatingViewModel.ApplicationUsers = _colibriDbContext.ApplicationUserRatings.ToList();

            // Search Conditions
            if (searchUserName != null)
            {
                ApplicationUserRatingViewModel.ApplicationUsers = ApplicationUserRatingViewModel.ApplicationUsers
                    .Where(a => a.ApplicationUserRatedName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }
            if (searchUserRating != 0)
            {
                ApplicationUserRatingViewModel.ApplicationUsers = ApplicationUserRatingViewModel.ApplicationUsers
                    .Where(a => a.ApplicationUserRate == searchUserRating).ToList();
            }

            // Pagination
            // count Users alltogether
            var count = ApplicationUserRatingViewModel.ApplicationUsers.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            ApplicationUserRatingViewModel.ApplicationUsers = ApplicationUserRatingViewModel.ApplicationUsers
                .OrderBy(p => p.ApplicationUserRatedName)
                .Skip((applicationUserPage - 1) * PageSize)
                .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            ApplicationUserRatingViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = applicationUserPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["ApplicationUserRatingList"] = _localizer["ApplicationUserRatingListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["RatedUser"] = _localizer["RatedUserText"];
            ViewData["RatingUser"] = _localizer["RatingUserText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];

            return View(ApplicationUserRatingViewModel);
        }

        // GET: Details
        [Route("Customer/ApplicationUserRating/Details/{string}")]
        public async Task<IActionResult> Details(string str)
        {
            if (str == "")
            {
                return NotFound();
            }

            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUserRatingViewModel.CurrentUserId = claim.Value;

            // get the User
            ApplicationUserRatingViewModel.ApplicationUser = await _colibriDbContext.ApplicationUserRatings
                                                .Where(p => p.ApplicationUserRatedId == str)
                                                .FirstOrDefaultAsync();

            // get the Application User

            // i18n
            ViewData["ApplicationUserDetails"] = _localizer["ApplicationUserDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["RatedUser"] = _localizer["RatedUserText"];
            ViewData["RatingUser"] = _localizer["RatingUserText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(ApplicationUserRatingViewModel);
        }

        [Route("Customer/ApplicationUserRating/Details/{string}")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(string str)
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Rating
            ApplicationUserRatingViewModel.CurrentUserId = claim.Value;

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                var userFromDb = await _colibriDbContext.ApplicationUserRatings
                    .Where(p => p.ApplicationUserRatedId == str)
                    .FirstOrDefaultAsync();

                _colibriDbContext.ApplicationUserRatings.Add(userFromDb);

                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ApplicationUserRatingViewModel);
            }
        }

        // Get: /<controller>/Edit
        [Route("Customer/ApplicationUserRating/Edit/{string}")]
        public async Task<IActionResult> Edit(string str)
        {
            if (str == "")
            {
                return NotFound();
            }

            // get the User
            ApplicationUserRatingViewModel.ApplicationUser = await _colibriDbContext.ApplicationUserRatings
                                                    .Where(p => p.ApplicationUserRatedId == str)
                                                    .FirstOrDefaultAsync();

            if (ApplicationUserRatingViewModel.ApplicationUsers == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditApplicationUserRating"] = _localizer["EditApplicationUserRatingText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(ApplicationUserRatingViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Customer/ApplicationUserRating/Edit/{string}")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string str)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // get the Product from the DB
                var userFromDb = await _colibriDbContext.ApplicationUserRatings
                                        .Where(p => p.ApplicationUserRatedId == str)
                                        .FirstOrDefaultAsync();

                userFromDb.ApplicationUserRatedName = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRatedName;
                userFromDb.ApplicationUserRatingName = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRatingName;
                userFromDb.ApplicationUserRate = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRate;
                userFromDb.Description = ApplicationUserRatingViewModel.ApplicationUser.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ApplicationUserRatingViewModel);
            }
        }

        // Get: /<controller>/Delete
        [Route("Customer/ApplicationUserRating/Delete/{id}")]
        public async Task<IActionResult> Delete(string str)
        {
            if (str == "")
            {
                return NotFound();
            }

            // get the User
            ApplicationUserRatingViewModel.ApplicationUser = await _colibriDbContext.ApplicationUserRatings
                                                            .Where(p => p.ApplicationUserRatedId == str)
                                                            .FirstOrDefaultAsync();

            if (ApplicationUserRatingViewModel.ApplicationUsers == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteApplicationUserRating"] = _localizer["DeleteApplicationUserRatingText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(ApplicationUserRatingViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Customer/ApplicationUserRating/Delete/{string}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string str)
        {
            ApplicationUserRatings applicationUserRatings = await _colibriDbContext.ApplicationUserRatings.FindAsync(str);

            if (applicationUserRatings == null)
            {
                return NotFound();
            }
            else
            {
                // remove the Entry from the DB
                _colibriDbContext.ApplicationUserRatings.Remove(applicationUserRatings);

                // save the Changes asynchronously
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Rating (extra to reference from the ProductsController)
        [Route("Customer/ApplicationUserRating/Rating/{str}")]
        public async Task<IActionResult> Rating(string str)
        {
            if (str != "")
            {
                return NotFound();
            }

            // get the User
            ApplicationUserRatingViewModel.ApplicationUser = await _colibriDbContext.ApplicationUserRatings
                                                            .Where(p => p.ApplicationUserRatedId == str)
                                                            .FirstOrDefaultAsync();

            // i18n
            ViewData["ApplicationUserDetails"] = _localizer["ApplicationUserDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(ApplicationUserRatingViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Customer/ApplicationUserRating/Rating/{string}")]
        [HttpPost, ActionName("Rating")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RatingPost(string str)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // get the Product from the DB
                var userFromDb = await _colibriDbContext.ApplicationUserRatings
                                        .Where(p => p.ApplicationUserRatedId == str)
                                        .FirstOrDefaultAsync();

                userFromDb.ApplicationUserRatedName = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRatedName;
                userFromDb.ApplicationUserRatingName = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRatingName;
                userFromDb.ApplicationUserRate = ApplicationUserRatingViewModel.ApplicationUser.ApplicationUserRate;
                userFromDb.Description = ApplicationUserRatingViewModel.ApplicationUser.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ApplicationUserRatingViewModel);
            }
        }
    }
}