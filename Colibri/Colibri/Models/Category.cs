using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class Categories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        [Required, MaxLength(50)]
        public string Name { get; set; }

        public int OfferId { get; set; }
        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }

        // Children Classes
        // Category Type #1: Product
        //public Product Product { get; set; }
        //// Category Type #2: Service
        //public Service Service { get; set; }
    }
}
