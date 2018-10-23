using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        /*
         * Foreign References
         */
        // #1 Product Type
        [Display(Name = "Product Type")]
        public int CategoryTypeId { get; set; }

        // 1 Product = 1 Category Type
        // 'virtual': not added to the DB
        [ForeignKey("CategoryTypeId")]
        public virtual CategoryTypes CategoryTypes { get; set; }

        // #2 Special Tag
        [Display(Name = "Special Tag")]
        public int SpecialTagId { get; set; }

        // 1 Product = 1 Category Type
        // 'virtual': not added to the DB
        [ForeignKey("SpecialTagId")]
        public virtual SpecialTags SpecialTags { get; set; }
    }
}
