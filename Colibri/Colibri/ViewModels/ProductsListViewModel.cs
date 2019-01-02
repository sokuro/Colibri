using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class ProductsListViewModel
    {
        public List<Products> Products { get; set; }
        public Products Product { get; set; }
        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }
    }
}
