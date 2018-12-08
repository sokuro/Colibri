using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class CategoryGroups
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        // Timestamp, wann die Rubrik angelegt wurde
        [Display(Name = "Erstellt am")]
        public DateTime CreatedOn { get; set; }

        // Product/Services
        [Required]
        public string TypeOfCategoryGroup { get; set; }

        public enum ETypeOfCategoryGroup
        {
            Products = 0,
            Services = 1
        }
    }
}
