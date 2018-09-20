using Colibri.Models;
using Colibri.Models.Category;
using Colibri.Models.Category.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * Model Class for the extended View
     */
    public class HomeIndexViewModel
    {
        // IEnumerable allows Iteration through Entities
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Transport> Transports { get; set; }
    }
}
