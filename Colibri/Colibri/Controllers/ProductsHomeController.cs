using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    public class ProductsHomeController : Controller
    {
        private readonly IColibriRepository _repository;

        public ProductsHomeController(IColibriRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}