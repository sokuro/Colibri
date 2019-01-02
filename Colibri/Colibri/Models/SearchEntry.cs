using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class SearchEntry
    {
        [Required]
        public int Id { get; set; }

        public string SearchText { get; set; }

        public DateTime SearchDate { get; set; }

        // Flags zum prüfen, ob Suche erfolgreich war
        public bool FullSuccess { get; set; }
        public bool PartSuccess { get; set; }
        public bool NoSuccess { get; set; }
        public int Counter { get; set; }
        public bool SearchOffer { get; set; }
    }
}
