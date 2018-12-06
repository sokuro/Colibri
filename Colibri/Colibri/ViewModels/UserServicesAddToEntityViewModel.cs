using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    /*
     * ViewModel necessary to persist a User Service
     *
     * used: Create, Delete
     */
    public class UserServicesAddToEntityViewModel
    {
        public UserServices UserServices { get; set; }
        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }
    }
}
