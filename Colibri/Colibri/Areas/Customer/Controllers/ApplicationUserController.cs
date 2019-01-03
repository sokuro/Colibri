using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Colibri.Areas.Admin.Controllers;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

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
        private readonly IStringLocalizer<ApplicationUserController> _localizer;

        [BindProperty]
        public ApplicationUserViewModel ApplicationUserViewModel { get; set; }

        // CTOR
        // get the Data from the DB
        public ApplicationUserController(ColibriDbContext colibriDbContext,
            IEmailSender emailSender,
            IStringLocalizer<ApplicationUserController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        // extend the Method with the Parameters for Search:
        // UserName, FirstName, LastName
        [Route("Customer/ApplicationUser/Index")]
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
            // DO NOT display SuperAdmins!
            applicationUserViewModel.ApplicationUsers = await _colibriDbContext.ApplicationUsers
                                                                .Where(u => u.IsSuperAdmin == false && u.UserName != "admin")
                                                                .ToListAsync();

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

            // i18n
            ViewData["ApplicationUsers"] = _localizer["ApplicationUsersText"];
            ViewData["NewApplicationUser"] = _localizer["NewApplicationUserText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["FirstName"] = _localizer["FirstNameText"];
            ViewData["LastName"] = _localizer["LastNameText"];
            ViewData["Search"] = _localizer["SearchText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberText"];
            ViewData["Disabled"] = _localizer["DisabledText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];

            // return the List of registered Application Users
            return View(applicationUserViewModel);
        }

        // Method Details GET
        [Route("Customer/ApplicationUser/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // get the individual User
            var user = await _colibriDbContext.ApplicationUsers
                            .Where(u => u.Id == id)
                            .FirstOrDefaultAsync();

            // i18n
            ViewData["ApplicationUserDetails"] = _localizer["ApplicationUserDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["FirstName"] = _localizer["FirstNameText"];
            ViewData["LastName"] = _localizer["LastNameText"];
            ViewData["Contact"] = _localizer["ContactText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserRating"] = _localizer["UserRatingText"];

            return View(user);
        }

        // Send Mail by Contact
        // GET: /<controller>/Contact
        [HttpGet("contactUser")]
        public ActionResult ContactUser()
        {
            // i18n
            ViewData["YourName"] = _localizer["YourNameText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["Subject"] = _localizer["SubjectText"];
            ViewData["Message"] = _localizer["MessageText"];
            ViewData["SendMessage"] = _localizer["SendMessageText"];
            ViewData["MessageSent"] = _localizer["MessageSentText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

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
                //ViewBag.UserMessage = _localizer["MessageSentText"];
                // clear the Model
                ModelState.Clear();
            }
            return RedirectToAction("Index", "ApplicationUser", new { area = "Customer" });
        }

        // Rate the User
        //[Route("Customer/ApplicationUser/RateUser/{id}")]
        //[HttpPost, ActionName("RateUser")]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RateUserPost(string id, string command)
        //{
        //    // Check the State Model Binding
        //    if (ModelState.IsValid)
        //    {
        //        // Security Claims
        //        System.Security.Claims.ClaimsPrincipal currentUser = this.User;

        //        // Claims Identity
        //        var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        //        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //        // to overwrite a Rating, first get the old One
        //        // get the User from the DB
        //        var userFromDb = await _colibriDbContext.ApplicationUsers
        //                                .Where(m => m.Id == id)
        //                                .FirstOrDefaultAsync();

        //        var userRatingFromDb = await _colibriDbContext.ApplicationUsersRatings
        //            .Where(p => p.ApplicationUserRatingId == id)
        //            .FirstOrDefaultAsync();

        //        // current User
        //        var currentUserId = claim.Value;

        //        if (userRatingFromDb != null)
        //        {
        //            // check, if already rated
        //            if (userRatingFromDb.ApplicationUserId == currentUserId)
        //            {
        //                // already rated!
        //                //userAlreadyRated = true;

        //                TempData["msg"] = "<script>alert('Already rated!');</script>";
        //                TempData["returnButton"] = "<div><p><b>Already rated!</b></p></div>";
        //                TempData["returnBackButton"] = "return";
        //                TempData["showProductRating"] = "showProductRating";
        //                TempData["productId"] = userFromDb.Id;

        //                ViewData["BackToList"] = _localizer["BackToListText"];
        //                ViewData["ShowAllRatings"] = _localizer["ShowAllRatingsText"];
        //                ViewData["ShowRating"] = _localizer["ShowRatingText"];

        //                return View();
        //            }
        //            else
        //            {
        //                int tempProductRating = 0;

        //                if (command.Equals("1"))
        //                {
        //                    tempProductRating = 1;
        //                }
        //                else if (command.Equals("2"))
        //                {
        //                    tempProductRating = 2;
        //                }
        //                else if (command.Equals("3"))
        //                {
        //                    tempProductRating = 3;
        //                }
        //                else if (command.Equals("4"))
        //                {
        //                    tempProductRating = 4;
        //                }
        //                else if (command.Equals("5"))
        //                {
        //                    tempProductRating = 5;
        //                }

        //                // go to the Product Table
        //                // calculate the new ProductRating
        //                if (userRatingFromDb.NumberOfProductRates == 0)
        //                {
        //                    userFromDb.ProductRating = tempProductRating;
        //                }
        //                else
        //                {
        //                    userFromDb.ProductRating = Math.Round((userFromDb.ProductRating * userFromDb.NumberOfProductRates + tempProductRating) / (userFromDb.NumberOfProductRates + 1), 2);
        //                }

        //                // Rating Create
        //                ProductsRatings productsRatings = new ProductsRatings()
        //                {
        //                    ProductId = userFromDb.Id,
        //                    ProductName = userFromDb.Name,
        //                    //ApplicationUserId = productFromDb.ApplicationUserId,
        //                    // add the current User as the Creator of the Rating
        //                    ApplicationUserId = claim.Value,
        //                    ApplicationUserName = claim.Subject.Name,
        //                    ProductRating = tempProductRating,
        //                    CreatedOn = System.DateTime.Now
        //                };

        //                // update the ProductsRatings Entity
        //                _colibriDbContext.ProductsRatings.Add(productsRatings);

        //                // increment the Number of Product Rates of the Product
        //                userFromDb.NumberOfProductRates += 1;

        //                // save the Changes in DB
        //                await _colibriDbContext.SaveChangesAsync();
        //            }

        //            return View(ApplicationUserViewModel);
        //        }

        //        //else if (productRatingFromDb == null && !userAlreadyRated)
        //        else
        //        {
        //            int tempProductRating = 0;

        //            if (command.Equals("1"))
        //            {
        //                tempProductRating = 1;
        //            }
        //            else if (command.Equals("2"))
        //            {
        //                tempProductRating = 2;
        //            }
        //            else if (command.Equals("3"))
        //            {
        //                tempProductRating = 3;
        //            }
        //            else if (command.Equals("4"))
        //            {
        //                tempProductRating = 4;
        //            }
        //            else if (command.Equals("5"))
        //            {
        //                tempProductRating = 5;
        //            }

        //            // go to the Product Table
        //            // calculate the new ProductRating
        //            if (userFromDb.NumberOfProductRates == 0)
        //            {
        //                userFromDb.ProductRating = tempProductRating;
        //            }
        //            else
        //            {
        //                userFromDb.ProductRating = Math.Round((userFromDb.ProductRating * userFromDb.NumberOfProductRates + tempProductRating) / (userFromDb.NumberOfProductRates + 1), 2);
        //            }

        //            // Rating Create
        //            ProductsRatings productsRatings = new ProductsRatings()
        //            {
        //                ProductId = userFromDb.Id,
        //                ProductName = userFromDb.Name,
        //                //ApplicationUserId = productFromDb.ApplicationUserId,
        //                // add the current User as the Creator of the Rating
        //                ApplicationUserId = claim.Value,
        //                ApplicationUserName = claim.Subject.Name,
        //                ProductRating = tempProductRating,
        //                CreatedOn = System.DateTime.Now
        //            };

        //            // update the ProductsRatings Entity
        //            _colibriDbContext.ProductsRatings.Add(productsRatings);

        //            // increment the Number of Product Rates of the Product
        //            userFromDb.NumberOfProductRates += 1;

        //            // save the Changes in DB
        //            await _colibriDbContext.SaveChangesAsync();
        //        }

        //        return RedirectToAction(nameof(Details));
        //    }
        //    else
        //    {
        //        // one can simply return to the Form View again for Correction
        //        return View(ApplicationUserViewModel);
        //    }
        //}
    }
}