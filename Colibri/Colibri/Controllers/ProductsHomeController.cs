using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    /*
     * Controller for the Products View
     */
    //[Authorize(Roles = "Administrator, PowerUser")]
    public class ProductsHomeController : Controller
    {
        private readonly IColibriRepository _repository;

        public ProductsHomeController(IColibriRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var productList = _repository.GetAllProductsAsync();

            return View(productList);
        }
    }
}