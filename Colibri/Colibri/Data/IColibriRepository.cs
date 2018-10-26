using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    /*
     * An extra Layer between the actual DB
     * 
     * 1) different Calls possible, without manipulating the DB
     * 2) all Queries in one Place (testing effectively)
     */
    public interface IColibriRepository
    {
        IEnumerable<Products> GetAllProducts();
        Task<IEnumerable<Products>> GetAllProductsAsync();
        //IEnumerable<Product> GetProductsByCategory(int category);
        Products GetProductById(int id);

        IEnumerable<Order> GetAllOrders(bool includeItems);
        IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems);
        Order GetOrderById(string username, int id);

        bool SaveAll();
        void AddEntity(object model);
    }
}
