using Colibri.Models;
using System.Collections.Generic;

namespace Colibri.ViewModels
{
    /*
     * Model Class for the extended View
     */
    public class HomeIndexViewModel
    {
        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public string SearchAdvertisement { get; set; }
    }
}
