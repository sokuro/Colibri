using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * ViewModel to handle the Application Users
     */
    public class ApplicationUserViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }

        // List of Users
        public List<ApplicationUser> ApplicationUsers { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }

        public string CurrentUserId { get; set; }
    }
}
