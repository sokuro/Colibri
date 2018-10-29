using Colibri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Colibri.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        [MinLength(4)]
        public string OrderNumber { get; set; }

        // to allow returning of the Data Collection
        public ICollection<OrderItemViewModel> Items { get; set; }

        // allow the User to make an Order
        public ApplicationUser OrderUser { get; set; }
    }
}