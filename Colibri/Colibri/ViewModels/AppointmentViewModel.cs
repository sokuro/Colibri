using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * ViewModel to handle the User Appointments
     */
    public class AppointmentViewModel
    {
        // List of Appointments
        public List<Appointments> Appointments { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }

        public string CurrentUserId { get; set; }

        public IEnumerable<ApplicationUser> Customer { get; set; }

        public string CustomerId { get; set; }
    }
}
