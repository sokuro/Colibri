using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Colibri.Data
{
    /*
     * simple API to get Data from the DB
     */
    public class ColibriRepository : IColibriRepository
    {
        private readonly ColibriDbContext _ctx;
        private readonly ILogger<ColibriRepository> _logger;

        // ColibriContext injected
        public ColibriRepository(ColibriDbContext ctx, ILogger<ColibriRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            else
            {
                return _ctx.Orders.ToList();
            }
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders
                    .Where(o => o.OrderUser.UserName == username)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            else
            {
                return _ctx.Orders
                    .Where(o => o.OrderUser.UserName == username)
                    .ToList();
            }
        }

        // returning Data (List of Products)
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                // use the Logger
                _logger.LogInformation("GetAllProducts was called");

                return _ctx.Products
                    .OrderBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }

        // Get individual Order
        public Order GetOrderById(string username, int id)
        {
            return _ctx.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.OrderId == id && o.OrderUser.UserName == username)
                .FirstOrDefault();
        }

        // return Products by Category
        public IEnumerable<Product> GetProductsByCategory(int category)
        {
            try
            {
                _logger.LogInformation("GetProductsByCategory was called");

                return _ctx.Products
                    .Where(p => p.CategoryId == category)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products by category: {ex}");
                return null;
            }
        }

        // save all Entries
        public bool SaveAll()
        {
            try
            {
                _logger.LogInformation("SaveAll was called");

                return _ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save the Products: {ex}");
                return false;
            }
        }
    }
}
