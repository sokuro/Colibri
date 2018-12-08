using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class CategoryTypes
    {
        // Einzelne Rubriken
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Timestamp, wann die Rubrik angelegt wurde
        [Display(Name = "Erstellt am")]
        public DateTime CreatedOn { get; set; }

        public bool isActive { get; set; }

        // Gültigkeit via PLZ
        public bool isGlobal { get; set; }

        public string PLZ { get; set; }

        // Fremdschlüssel für Rubrik-Gruppe
        [Required]
        [Display(Name = "Rubrik-Gruppe")]
        public int CategoryGroupId { get; set; }

        [ForeignKey("CategoryGroupId")]
        public virtual CategoryGroups CategoryGroups { get; set; }

    }
}
