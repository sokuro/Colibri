using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string TypeOfAdvertisement { get; set; }

        // Rubrik
        public CategoryTypes CategoryTypes { get; set; }

        // Rubrik-Gruppe
        public CategoryGroups CategoryGroups { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
