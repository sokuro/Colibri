using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class ProductsRatingViewModel
    {
        public List<ProductsRatings> Products { get; set; }
        public ProductsRatings Product { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public string CurrentUserId { get; set; }
    }
}
