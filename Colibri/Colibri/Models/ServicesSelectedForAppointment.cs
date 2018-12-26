using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    /*
     * Entity as Connector between the Services and the User's, resp. the Order's Appointment
     */
    public class ServicesSelectedForAppointment
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public virtual Appointments Appointments { get; set; }

        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual UserServices UserServices { get; set; }
    }
}