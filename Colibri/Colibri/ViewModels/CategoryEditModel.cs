using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * Class to avoid 'OverPOSTing' in the HTML-Form
     * 
     * Fields that match with the Names in the Form
     */
    public class CategoryEditModel
    {
        [Required(ErrorMessage = "{0} must be specified!"),MaxLength(50)]
        public string Name { get; set; }
    }
}
