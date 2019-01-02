using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class UserSearchesViewModel
    {
        public IEnumerable<SearchEntry> SearchEntryList { get; set; }


        public SearchEntry SearchEntry { get; set; }

        // Boolean-Werte für Alle / Erfolg / Teilerfolg / Kein Erfolg
        public bool resAll { get; set; }
        public bool resSuccess { get; set; }
        public bool resPartSuccess { get; set; }
        public bool resNoSuccess { get; set; }

        // Boolean-Werte für Zeitraum für Alle / 30 Tage / Heute
        public bool dateAll { get; set; }
        public bool date30Days { get; set; }
        public bool dateToday { get; set; }

        // Prüfen, ob Suche nach Angebot oder Nachfrage
        public bool searchAll { get; set; }
        public bool searchOffer { get; set; }
        public bool searchRequest { get; set; }

        // Zähler gefundene Einträge
        public int ResultsCounter { get; set; }
    }
}
