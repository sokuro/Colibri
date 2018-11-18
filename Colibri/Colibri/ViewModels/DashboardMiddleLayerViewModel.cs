using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * Dashboard Middle Layer with:
     * 
     * #1: Categories
     * #2: Products
     */
    public class DashboardMiddleLayerViewModel
    {
        public List<CategoryTypes> Categories { get; set; }
        public List<Products> Products { get; set; }
    }
}
