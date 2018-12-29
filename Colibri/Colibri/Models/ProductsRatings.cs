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
     * Entity as Connector between the Products and the User's Rating
     */
    public class ProductsRatings
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

        public string ApplicationUserId { get; set; }

        // 1 Product = 1 User
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserName { get; set; }

        public int ProductRating { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
