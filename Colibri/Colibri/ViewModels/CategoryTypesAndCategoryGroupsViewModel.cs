using Colibri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class CategoryTypesAndCategoryGroupsViewModel
    {
        // Rubriken
        public CategoryTypes CategoryTypes { get; set; }

        // Liste mit allen verfügbaren Rubrik-Gruppen
        public IEnumerable<CategoryGroups> CategoryGroupsList { get; set; }

        // Rubrik-Gruppe
        public CategoryGroups CategoryGroups { get; set; }
        public string CategoryGroupsCombined { get; set; }

        // Liste mit allen Rubriken
        public List<string> CategoryTypesList { get; set; }

        public IEnumerable<CategoryTypes> CategoryTypesListE { get; set; }

        [Display(Name = "Neue Rubrik")]
        public bool isNew { get; set; }

        // String StatussMessage wird verwendet, um Fehlermeldungen anzuzeigen
        public String StatusMessage { get; set; }
    }
}
