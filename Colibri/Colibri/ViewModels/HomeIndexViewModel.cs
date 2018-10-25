using Colibri.Models;
using System.Collections.Generic;

namespace Colibri.ViewModels
{
    /*
     * Model Class for the extended View
     */
    public class HomeIndexViewModel
    {
        // IEnumerable allows Iteration through Entities
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
