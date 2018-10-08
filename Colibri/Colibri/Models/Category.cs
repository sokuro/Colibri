﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        [Required, MaxLength(50)]
        public string Name { get; set; }

        // Category Type #1: Product
        //public ICollection<Product> Products { get; set; }
        //// Category Type #1: Service
        //public ICollection<Service> Services { get; set; }
    }
}
