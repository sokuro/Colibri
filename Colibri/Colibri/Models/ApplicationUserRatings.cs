using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class ApplicationUserRatings
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ApplicationUserRatedId")]
        public virtual ApplicationUser ApplicationUserRated { get; set; }
        public string ApplicationUserRatedName { get; set; }
        public string ApplicationUserRatedId { get; set; }

        [ForeignKey("ApplicationUserRatingId")]
        public virtual ApplicationUser ApplicationUserRating { get; set; }
        public string ApplicationUserRatingId { get; set; }
        public string ApplicationUserRatingName { get; set; }

        public int ApplicationUserRate { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
