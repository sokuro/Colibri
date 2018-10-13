using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        // relationship to the Entity 'product'
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // relationship to the Entity 'order'
        public Order Order { get; set; }
    }
}
