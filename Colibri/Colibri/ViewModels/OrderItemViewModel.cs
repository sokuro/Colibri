using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        // extend the Properties of the individual Order's Item
        [Required]
        public int ProductId { get; set; }

        //public string ProductCategory { get; set; }
    }

}
