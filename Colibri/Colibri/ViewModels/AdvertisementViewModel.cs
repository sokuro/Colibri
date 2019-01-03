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
        // Liste mit Güter
        public List<Products> Products { get; set; }
        public Products Product { get; set; }

        // Liste mit Dienstleistungen
        public List<UserServices> UserServices { get; set; }
        public UserServices UserService { get; set; }

        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }

        // for Filtering
        public string SearchFilter { get; set; }

        public string CurrentUserId { get; set; }

        public string OwnerId { get; set; }
    }
}
