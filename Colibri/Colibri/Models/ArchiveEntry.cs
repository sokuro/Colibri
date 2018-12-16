using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class ArchiveEntry
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        // Angebot oder Nachfrage
        [Required]
        public bool isOffer { get; set; }

        // Produkt oder Dienstleistung
        // "Product" / "Service"
        [Required]
        public string TypeOfCategoryGroup { get; set; }

        public DateTime CreatedOn { get; set; }

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


    }
}
