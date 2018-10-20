using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Colibri.Data;
using Colibri.Models;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        // CTOR:
        // 1) Repository to deal with the Data
        // 2) Logger
        public ProductsController(IColibriRepository repository, 
            ILogger<ProductsController> logger,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        // ActionMethod of the API
        // (custom) Annotation: Response Type of the Method
        [HttpGet]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public ActionResult<IEnumerable<Product>> Get()
        public ActionResult Get()
        //public JsonResult Get()
        {
            try
            {
                var products = _repository.GetAllProducts();
                // wrap the result in JSON
                //return Ok(_repository.GetAllProducts());
                //return Json(_repository.GetAllProducts());
                return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(products));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get products: {ex}");
                return BadRequest("Failed request");
                //return Json("Bad Request");
            }
        }

        // Get an individual Produrct
        // map explicitly integers
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var product = _repository.GetProductById(id);

                if (product != null)
                {
                    // use AutoMapper to wrap the Product into the ProductViewModel
                    // it takes the Product and returns the MapVersion of it
                    return Ok(_mapper.Map<Product, ProductViewModel>(product));
                }
                else
                {
                    // returning 'NotFound' instead of 'BadRequest'
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get product by ID: {ex}");
                return BadRequest("Failed to get product by ID");
            }
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        // Show some Product Offers
        // authorize
        //[Authorize]
        public IActionResult ShowProductOffers()
        {
            return View();
        }

        // Make an Product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ProductViewModel model)
        {
            // add to the DB
            try
            {
                if (ModelState.IsValid)
                {
                    // use AutoMapper for the newProduct Model to the Entity
                    var newProduct = _mapper.Map<ProductViewModel, Product>(model);

                    // before the Entity is added in, define the User
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newProduct.OfferUser = currentUser;

                    _repository.AddEntity(newProduct);

                    if (_repository.SaveAll())
                    {
                        // AutoMap back from the newProduct -> ViewModel
                        // POST Requirement: return "Created"
                        return Created($"/api/products/{newProduct.ProductId}", _mapper.Map<Product, ProductViewModel>(newProduct));
                    }
                }
                else
                {
                    // return the ModelState in the BadRequest for Errors
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new Product: {ex}");
            }

            return BadRequest("Failed to save the new Product");
        }
    }
}