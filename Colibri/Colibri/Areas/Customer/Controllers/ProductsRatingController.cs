using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
     * Controller to Handle Product's Ratings
     */
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class ProductsRatingController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<ProductsRatingController> _localizer;

        [BindProperty]
        public ProductsRatingViewModel ProductsRatingViewModel { get; set; }

        public ProductsRatingController(ColibriDbContext colibriDbContext,
            IStringLocalizer<ProductsRatingController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            ProductsRatingViewModel = new ProductsRatingViewModel()
            {
                Products = new List<ProductsRatings>(),
                Product = new Models.ProductsRatings(),
                Users = new List<ApplicationUser>()
            };
        }

        [Route("Customer/ProductsRating/Index")]
        public IActionResult Index()
        {
            ProductsRatingViewModel.Products = _colibriDbContext.ProductsRatings.ToList();

            return View(ProductsRatingViewModel);
        }


        // GET: Details
        [Route("Customer/ProductsRating/Details/{id}")]
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
            ProductsRatingViewModel.CurrentUserId = claim.Value;

            // get the Product
            ProductsRatingViewModel.Product = await _colibriDbContext.ProductsRatings
                                                .Where(p => p.Id == id)
                                                .FirstOrDefaultAsync();

            // get the Application User

            return View(ProductsRatingViewModel);
        }

        [Route("Customer/ProductsRating/Details/{id}")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Rating
            ProductsRatingViewModel.CurrentUserId = claim.Value;

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                var productFromDb = _colibriDbContext.ProductsRatings.Where(p => p.Id == id);

                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ProductsRatingViewModel);
            }
        }
    }
}