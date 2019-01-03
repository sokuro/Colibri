using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class ApplicationUserRatingViewModel
    {
        public ApplicationUserRatings ApplicationUser { get; set; }

        // List of Users
        public List<ApplicationUserRatings> ApplicationUsers { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }

        public string CurrentUserId { get; set; }
    }
}
