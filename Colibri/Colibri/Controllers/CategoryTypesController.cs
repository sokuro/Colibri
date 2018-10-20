using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore.Mvc;

namespace Colibri.Controllers
{
    public class CategoryTypesController : Controller
    {
        private ColibriDbContext _db;

        public CategoryTypesController(ColibriDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.CategoryTypes.ToList());
        }
    }
}