using Colibri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class SearchViewModel
    {
        [Display(Name = "Rubrik Typ")]
        public string SearchCategoryType { get; set; }

        [Display(Name = "Rubrik Gruppe")]
        public string SearchCategoryGroup { get; set; }

        [Display(Name = "Rubrik")]
        public string SearchCategory { get; set; }

        [Display(Name = "Inserat Titel")]
        public string SearchAdvertisement { get; set; }

        public string SearchAdvertisementRequest { get; set; }

        // Güter
        public Products Products{ get; set; }
        public IEnumerable<Products> ProductsList { get; set; }

        // Dienstleistungen
        public UserServices UserServices { get; set; }
        public IEnumerable<UserServices> UserServicesList { get; set; }

        // Angebot oder Nachfrage
        public string TypeOfAdvertisement { get; set; }

        // Rubrik-Gruppe
        public IEnumerable<CategoryGroups> CategoryGroupsList { get; set; }

        // Rubrik
        public IEnumerable<CategoryTypes> CategoryTypesList { get; set; }

        public IEnumerable<ArchiveEntry> ArchiveEntryList { get; set; }

        public int ResultsCounter { get; set; }
        public int ResultsCounterArchive { get; set; }
    }
}
