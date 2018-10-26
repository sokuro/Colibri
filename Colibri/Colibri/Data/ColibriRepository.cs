using Colibri.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colibri.Data
{
    /*
     * simple API to get Data from the DB
     */
    public class ColibriRepository : IColibriRepository
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly ILogger<ColibriRepository> _logger;

        // ColibriContext injected
        public ColibriRepository(ColibriDbContext colibriDbContext, ILogger<ColibriRepository> logger)
        {
            _colibriDbContext = colibriDbContext;
            _logger = logger;
        }

        // add Entity (generic)
        public void AddEntity(object model)
        {
            _colibriDbContext.Add(model);
        }

        // remove Entity (generic)
        public void RemoveEntity(object model)
        {
            _colibriDbContext.Remove(model);
        }

        /*
         * Products
         * *********
         */
        // returning Data (List of Products)
        public IEnumerable<Products> GetAllProducts()
        {
            try
            {
                // use the Logger
                _logger.LogInformation("GetAllProducts was called");

                return _colibriDbContext.Products
                    .OrderBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }

        // Get individual Product
        public Products GetProductById(int id)
        {
            try
            {
                // user the Logger
                _logger.LogInformation("GetProductById was called");

                return _colibriDbContext.Products
                    .Where(p => p.Id == id)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the specific Product: {ex}");
                return null;
            }
        }

        // return Products by Category
        //public IEnumerable<Product> GetProductsByCategory(int category)
        //{
        //    try
        //    {
        //        _logger.LogInformation("GetProductsByCategory was called");

        //        return _colibriDbContext.Products
        //            .Where(p => p.CategoryId == category)
        //            .ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Failed to get all products by category: {ex}");
        //        return null;
        //    }
        //}

        /*
         * Orders
         * ******
         */
        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return _colibriDbContext.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            else
            {
                return _colibriDbContext.Orders.ToList();
            }
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
        {
            if (includeItems)
            {
                return _colibriDbContext.Orders
                    .Where(o => o.OrderUser.UserName == username)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            else
            {
                return _colibriDbContext.Orders
                    .Where(o => o.OrderUser.UserName == username)
                    .ToList();
            }
        }

        // Get individual Order
        public Order GetOrderById(string username, int id)
        {
            try
            {
                // user the Logger
                _logger.LogInformation("GetOrderById was called");

                return _colibriDbContext.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(o => o.OrderId == id && o.OrderUser.UserName == username)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the specific Order: {ex}");
                return null;
            }
        }


        // save all Entries
        public bool SaveAll()
        {
            try
            {
                _logger.LogInformation("SaveAll was called");

                return _colibriDbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save the Products: {ex}");
                return false;
            }
        }
    }

}
