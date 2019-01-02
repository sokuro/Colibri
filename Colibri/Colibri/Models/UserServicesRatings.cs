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
     * Entity as Connector between the User Services and the User's Rating
     */
    public class UserServicesRatings
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserServiceId { get; set; }
        [ForeignKey("UserServiceId")]
        public virtual UserServices UserServices { get; set; }

        public string UserServiceName { get; set; }

        public string ApplicationUserId { get; set; }

        // 1 User Service = 1 User
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserName { get; set; }

        public int UserServiceRating { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
