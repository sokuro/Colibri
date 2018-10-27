using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    /*
     * Entity as Connector between the Products and the User, resp. the Order
     */
    public class ProductsSelectedForAppointment
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public virtual Appointments Appointments { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }
}
