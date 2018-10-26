using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Controllers
{
    /*
     * Controller for the Products View
     */
    //[Authorize(Roles = "Administrator, PowerUser")]
    public class ProductsHomeController : Controller
    {
        //private readonly IColibriRepository _repository;
        private readonly ColibriDbContext _colibriDbContext;

        //public ProductsHomeController(IColibriRepository repository)
        public ProductsHomeController(ColibriDbContext colibriDbContext)
        {
            //_repository = repository;
            _colibriDbContext = colibriDbContext;
        }

        public async Task<IActionResult> Index()
        {
            //var productList = _repository.GetAllProductsAsync();
            var productList = await _colibriDbContext.Products
                    .Include(p => p.CategoryTypes)
                    .Include(p => p.SpecialTags)
                    .ToListAsync();

            return View(productList);
        }
    }
}