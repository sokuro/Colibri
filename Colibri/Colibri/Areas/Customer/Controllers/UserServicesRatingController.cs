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
     * Controller to Handle UserServiceRating's Ratings
     */
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class UserServicesRatingController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<UserServicesRatingController> _localizer;

        // PageSize (for the Pagination: 10 UserServiceRating Ratings/Page)
        private int PageSize = 10;

        [BindProperty]
        public UserServicesRatingViewModel UserServicesRatingViewModel { get; set; }

        public UserServicesRatingController(ColibriDbContext colibriDbContext,
            IStringLocalizer<UserServicesRatingController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            UserServicesRatingViewModel = new UserServicesRatingViewModel()
            {
                UserServices = new List<UserServicesRatings>(),
                UserServiceRating = new Models.UserServicesRatings(),
                Users = new List<ApplicationUser>()
            };
        }

        [Route("Customer/UserServicesRating/Index")]
        public IActionResult Index(
            int userServicePage = 1,
            string searchUserName = null,
            string searchUserServiceName = null
            )
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Advertisement
            UserServicesRatingViewModel.CurrentUserId = claim.Value;

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/UserServicesRating/Index?userServicePage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchName=");
            if (searchUserServiceName != null)
            {
                param.Append(searchUserServiceName);
            }

            // populate the Lists
            UserServicesRatingViewModel.UserServices = _colibriDbContext.UserServicesRatings.ToList();

            // Search Conditions
            if (searchUserServiceName != null)
            {
                UserServicesRatingViewModel.UserServices = UserServicesRatingViewModel.UserServices
                    .Where(a => a.UserServiceName.ToLower().Contains(searchUserServiceName.ToLower())).ToList();
            }
            if (searchUserName != null)
            {
                UserServicesRatingViewModel.UserServices = UserServicesRatingViewModel.UserServices
                    .Where(a => a.ApplicationUserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }

            // Pagination
            // count Advertisements alltogether
            var count = UserServicesRatingViewModel.UserServices.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            UserServicesRatingViewModel.UserServices = UserServicesRatingViewModel.UserServices
                .OrderBy(p => p.UserServiceName)
                .Skip((userServicePage - 1) * PageSize)
                .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            UserServicesRatingViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = userServicePage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["UserServiceRatingList"] = _localizer["UserServiceRatingListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["UserServiceRating"] = _localizer["UserServiceRatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["Search"] = _localizer["SearchText"];

            return View(UserServicesRatingViewModel);
        }

        // GET: Details
        [Route("Customer/UserServicesRating/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Advertisement
            UserServicesRatingViewModel.CurrentUserId = claim.Value;

            // get the UserService
            UserServicesRatingViewModel.UserServiceRating = await _colibriDbContext.UserServicesRatings
                                                .Where(p => p.Id == id)
                                                .FirstOrDefaultAsync();

            // get the Application User

            // i18n
            ViewData["UserServiceDetails"] = _localizer["UserServiceDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(UserServicesRatingViewModel);
        }

        [Route("Customer/UserServicesRating/Details/{id}")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            if (ModelState.IsValid)
            {

            }

            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Rating
            UserServicesRatingViewModel.CurrentUserId = claim.Value;

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                var userServiceFromDb = await _colibriDbContext.UserServicesRatings
                    .Where(p => p.UserServiceId == id)
                    .FirstOrDefaultAsync();

                _colibriDbContext.UserServicesRatings.Add(userServiceFromDb);

                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(UserServicesRatingViewModel);
            }
        }

        // Get: /<controller>/Edit
        [Route("Customer/UserServicesRating/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserServicesRatingViewModel.UserServiceRating = await _colibriDbContext.UserServicesRatings
                                                    .Where(p => p.Id == id)
                                                    .FirstOrDefaultAsync();

            if (UserServicesRatingViewModel.UserServices == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditUserServiceRating"] = _localizer["EditUserServiceRatingText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(UserServicesRatingViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Customer/UserServicesRating/Edit/{id}")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // get the UserService from the DB
                var userServiceFromDb = await _colibriDbContext.UserServicesRatings
                                        .Where(p => p.Id == id)
                                        .FirstOrDefaultAsync();

                userServiceFromDb.UserServiceName = UserServicesRatingViewModel.UserServiceRating.UserServiceName;
                userServiceFromDb.ApplicationUserName = UserServicesRatingViewModel.UserServiceRating.ApplicationUserName;
                userServiceFromDb.UserServiceRating = UserServicesRatingViewModel.UserServiceRating.UserServiceRating;
                userServiceFromDb.Description = UserServicesRatingViewModel.UserServiceRating.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(UserServicesRatingViewModel);
            }
        }

        // Get: /<controller>/Delete
        [Route("Customer/UserServicesRating/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserServicesRatingViewModel.UserServiceRating = await _colibriDbContext.UserServicesRatings
                                                .Where(p => p.Id == id)
                                                .FirstOrDefaultAsync();

            if (UserServicesRatingViewModel.UserServices == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteUserServiceRating"] = _localizer["DeleteUserServiceRatingText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(UserServicesRatingViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Customer/UserServicesRating/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            UserServicesRatings userServicesRatings = await _colibriDbContext.UserServicesRatings.FindAsync(id);

            if (userServicesRatings == null)
            {
                return NotFound();
            }
            else
            {
                // remove the Entry from the DB
                _colibriDbContext.UserServicesRatings.Remove(userServicesRatings);

                // save the Changes asynchronously
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Rating (extra to reference from the UserServiceController)
        [Route("Customer/UserServicesRating/Rating/{id}")]
        public async Task<IActionResult> Rating(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual User Service
            UserServicesRatingViewModel.UserServiceRating = await _colibriDbContext.UserServicesRatings
                                                                .Where(p => p.UserServiceId == id)
                                                                .FirstOrDefaultAsync();

            // i18n
            ViewData["UserServiceDetails"] = _localizer["UserServiceDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["UserServiceName"] = _localizer["UserServiceNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(UserServicesRatingViewModel);
        }
    }
}