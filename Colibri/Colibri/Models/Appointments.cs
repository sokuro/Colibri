using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    /*
     * Appointment Class to handle Appointments regarding Orders
     */
    public class Appointments
    {
        public int Id { get; set; }

        // Reference to an Application User
        [Display(Name = "Application User")]
        public string AppPersonId { get; set; }
        [ForeignKey("AppPersonId")]
        public virtual ApplicationUser AppPerson { get; set; }

        public DateTime AppointmentDate { get; set; }

        //only the AppointmentDate will be added to the DB
        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        // Customer Section
        [Display(Name = "Appointment Customer")]
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool isConfirmed { get; set; }

    }
}
