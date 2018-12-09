using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class AdvertisementViewModel
    {
        public List<Products> Products { get; set; }

        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }

        public string CurrentUserId { get; set; }
    }
}
