using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        // Bool Property to switch between Order or Offer
        public bool OfferOrder { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name="User")]
        //public int UserId { get; set; }

        // 1 Product = 1 Category
        [ForeignKey("CategoryId")]
        public Categories Categories { get; set; }

        // 1 Product = 1 User
        [ForeignKey("UserId")]
        public User OfferUser { get; set; }
    }
}
