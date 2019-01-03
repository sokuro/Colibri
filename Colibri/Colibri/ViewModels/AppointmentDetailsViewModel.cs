using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * ViewModel to handle Appointment Details
     */
    public class AppointmentDetailsViewModel
    {
        public Appointments Appointment { get; set; }
        public List<ApplicationUser> AppPerson { get; set; }
        public List<Products> Products { get; set; }
        public List<UserServices> UserServices { get; set; }
    }
}
