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
     * Controller to Handle Product's Ratings
     */
    //[Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Customer")]
    public class ProductsRatingController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<ProductsRatingController> _localizer;

        // PageSize (for the Pagination: 10 Product Ratings/Page)
        private int PageSize = 10;

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
        public IActionResult Index(
            int productPage = 1,
            string searchUserName = null,
            string searchProductName = null
            )
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Creator of the Advertisement
            ProductsRatingViewModel.CurrentUserId = claim.Value;

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/ProductsRating/Index?productPage=:");
            param.Append("&searchName=");
            if (searchUserName != null)
            {
                param.Append(searchUserName);
            }
            param.Append("&searchName=");
            if (searchProductName != null)
            {
                param.Append(searchProductName);
            }

            // populate the Lists
            ProductsRatingViewModel.Products = _colibriDbContext.ProductsRatings.ToList();

            // Search Conditions
            if (searchProductName != null)
            {
                ProductsRatingViewModel.Products = ProductsRatingViewModel.Products
                    .Where(a => a.ProductName.ToLower().Contains(searchProductName.ToLower())).ToList();
            }
            if (searchUserName != null)
            {
                ProductsRatingViewModel.Products = ProductsRatingViewModel.Products
                    .Where(a => a.ApplicationUserName.ToLower().Contains(searchUserName.ToLower())).ToList();
            }

            // Pagination
            // count Advertisements alltogether
            var count = ProductsRatingViewModel.Products.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            ProductsRatingViewModel.Products = ProductsRatingViewModel.Products
                .OrderBy(p => p.ProductName)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            // populate the PagingInfo Model
            // StringBuilder
            ProductsRatingViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // i18n
            ViewData["ProductsRatingList"] = _localizer["ProductsRatingListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];

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

            // i18n
            ViewData["ProductDetails"] = _localizer["ProductDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(ProductsRatingViewModel);
        }

        [Route("Customer/ProductsRating/Details/{id}")]
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
            ProductsRatingViewModel.CurrentUserId = claim.Value;

            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                var productFromDb = await _colibriDbContext.ProductsRatings
                    .Where(p => p.ProductId == id)
                    .FirstOrDefaultAsync();

                _colibriDbContext.ProductsRatings.Add(productFromDb);

                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ProductsRatingViewModel);
            }
        }

        // Get: /<controller>/Edit
        [Route("Customer/ProductsRating/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // search for the ID
            // incl. ProductTypes and SpecialTags too
            ProductsRatingViewModel.Product = await _colibriDbContext.ProductsRatings
                                                    .Where(p => p.Id == id)
                                                    .FirstOrDefaultAsync();

            if (ProductsRatingViewModel.Products == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["EditProductRating"] = _localizer["EditProductRatingText"];
            ViewData["Edit"] = _localizer["EditText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(ProductsRatingViewModel);
        }

        // Post: /<controller>/Edit
        // @param Category
        [Route("Customer/ProductsRating/Edit/{id}")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            // Check the State Model Binding
            if (ModelState.IsValid)
            {
                // get the Product from the DB
                var productFromDb = await _colibriDbContext.ProductsRatings
                                        .Where(p => p.Id == id)
                                        .FirstOrDefaultAsync();

                productFromDb.ProductName = ProductsRatingViewModel.Product.ProductName;
                productFromDb.ApplicationUserName = ProductsRatingViewModel.Product.ApplicationUserName;
                productFromDb.ProductRating = ProductsRatingViewModel.Product.ProductRating;
                productFromDb.Description = ProductsRatingViewModel.Product.Description;

                // Save the Changes
                await _colibriDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // one can simply return to the Form View again for Correction
                return View(ProductsRatingViewModel);
            }
        }

        // Get: /<controller>/Delete
        [Route("Customer/ProductsRating/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsRatingViewModel.Product = await _colibriDbContext.ProductsRatings
                                                .Where(p => p.Id == id)
                                                .FirstOrDefaultAsync();

            if (ProductsRatingViewModel.Products == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["DeleteProductRatings"] = _localizer["DeleteProductRatingsText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["BackToList"] = _localizer["BackToListText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];

            return View(ProductsRatingViewModel);
        }

        // Post: /<controller>/Delete
        // @param Category
        [Route("Customer/ProductsRating/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ProductsRatings productsRatings = await _colibriDbContext.ProductsRatings.FindAsync(id);

            if (productsRatings == null)
            {
                return NotFound();
            }
            else
            {
                // remove the Entry from the DB
                _colibriDbContext.ProductsRatings.Remove(productsRatings);

                // save the Changes asynchronously
                await _colibriDbContext.SaveChangesAsync();

                // avoid Refreshing the POST Operation -> Redirect
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Rating (extra to reference from the ProductsController)
        [Route("Customer/ProductsRating/Rating/{id}")]
        public async Task<IActionResult> Rating(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get the individual Product
            ProductsRatingViewModel.Product = await _colibriDbContext.ProductsRatings
                                                    .Where(p => p.ProductId == id)
                                                    .FirstOrDefaultAsync();

            // i18n
            ViewData["ProductDetails"] = _localizer["ProductDetailsText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["ProductName"] = _localizer["ProductNameText"];
            ViewData["Rating"] = _localizer["RatingText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["ViewDetails"] = _localizer["ViewDetailsText"];
            ViewData["BackToList"] = _localizer["BackToListText"];

            return View(ProductsRatingViewModel);
        }
    }
}