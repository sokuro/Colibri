using Colibri.Models;
using Colibri.Models.Category;
using System.Collections.Generic;

namespace Colibri.ViewModels
{
    /*
     * Model Class for the extended View
     */
    public class HomeIndexViewModel
    {
        // IEnumerable allows Iteration through Entities
        public IEnumerable<Categories> Categories { get; set; }
        public IEnumerable<Offer> Offers { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
