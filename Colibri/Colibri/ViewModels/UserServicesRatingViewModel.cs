using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class UserServicesRatingViewModel
    {
        public List<UserServices> UserServices { get; set; }
        public UserServicesRatings UserServiceRating { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public string CurrentUserId { get; set; }
    }
}
