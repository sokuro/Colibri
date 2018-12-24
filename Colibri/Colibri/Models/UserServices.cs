using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class UserServices
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        //public double Duration { get; set; }

        public DateTime DueDateFrom { get; set; }
        public DateTime DueDateTo { get; set; }
        public DateTime CreatedOn { get; set; }

        // Number of Clicks on the Product
        public int NumberOfClicks { get; set; }

        // Rating of the Service
        public int NumberOfServiceRates { get; set; }
        public double ServiceRating { get; set; }

        [Required]
        public bool isOffer { get; set; }

        /*
         * Foreign References
         */
        // #1 Category Group
        [Display(Name = "Category Group")]
        public int CategoryGroupId { get; set; }

        // 1 Product = 1 Category Type
        // 'virtual': not added to the DB
        [ForeignKey("CategoryGroupId")]
        public virtual CategoryGroups CategoryGroups { get; set; }

        // #2 Category Type
        [Display(Name = "Category Type")]
        public int CategoryTypeId { get; set; }

        // 1 Product = 1 Category Type
        // 'virtual': not added to the DB
        [ForeignKey("CategoryTypeId")]
        public virtual CategoryTypes CategoryTypes { get; set; }

        //// #3 Special Tag
        //[Display(Name = "Special Tag")]
        //public int SpecialTagId { get; set; }

        //// 'virtual': not added to the DB
        //[ForeignKey("SpecialTagId")]
        //public virtual SpecialTags SpecialTags { get; set; }

        // #4 User
        public string ApplicationUserId { get; set;}

        // 1 Product = 1 User
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
