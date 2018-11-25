using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class UserServicesViewModel
    {
        public List<UserServices> UserServices { get; set; }

        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
