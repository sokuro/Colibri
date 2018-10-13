using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Colibri.Controllers
{
    // ApiController Type
    // returning Value always a JSON
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductsController : Controller
    {
        private readonly IColibriRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        // CTOR:
        // 1) Repository to deal with the Data
        // 2) Logger
        public ProductsController(IColibriRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // ActionMethod of the API
        // (custom) Annotation: Response Type of the Method
        [HttpGet]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public ActionResult<IEnumerable<Product>> Get()
        public JsonResult Get()
        {
            try
            {
                // wrap the result in JSON
                //return Ok(_repository.GetAllProducts());
                return Json(_repository.GetAllProducts());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get products: {ex}");
                //return BadRequest("Failed request");
                return Json("Bad Request");
            }
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}