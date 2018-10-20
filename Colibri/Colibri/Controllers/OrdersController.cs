using AutoMapper;
using Colibri.Data;
using Colibri.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Colibri.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IColibriRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public OrdersController(IColibriRepository repository,
            ILogger<OrdersController> logger, 
            IMapper mapper,
            UserManager<User> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Get the Collection of Orders
        // extend the Method with the Possibility to includeItems
        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                // the User is known through the Authorization
                var username = User.Identity.Name;

                // create an Object 'results' for Mapping
                var results = _repository.GetAllOrdersByUser(username, includeItems);

                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(_repository.GetAllOrders(includeItems)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        // Get an individual Order
        // map explicitly integers
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, id);

                if (order != null)
                {
                    // use AutoMapper to wrap the Order into the OrderViewModel
                    // it takes the Order and returns the MapVersion of it
                    return Ok(_mapper.Map<Order, OrderViewModel>(order));
                }
                else
                {
                    // returning 'NotFound' instead of 'BadRequest'
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        // Make an Order
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            // add to the DB
            try
            {
                if (ModelState.IsValid)
                {
                    // use AutoMapper for the newOrder Model to the Entity
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    // add Validation: in Case the Date was not specified
                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    // before the Entity is added in, define the User
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.OrderUser = currentUser;

                    _repository.AddEntity(newOrder);

                    if (_repository.SaveAll())
                    {
                        // AutoMap back from the newOrder -> ViewModel
                        // POST Requirement: return "Created"
                        return Created($"/api/orders/{newOrder.OrderId}", _mapper.Map<Order, OrderViewModel>(newOrder));
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
                _logger.LogError($"Failed to save a new Order: {ex}");
            }

            return BadRequest("Failed to save the new Order");
        }
    }

}