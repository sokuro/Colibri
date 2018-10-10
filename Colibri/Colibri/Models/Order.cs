using Colibri.Models.Category;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        //ICollection<Product> Products { get; set; }

        // allow the User to make an Order
        public User OrderUser { get; set; }

        // Category Reference
        //public virtual ICollection<Category> Categories { get; set; }
    }
}
