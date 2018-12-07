﻿using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    /*
     * Shopping Cart View Model with:
     * #1: Products
     * #2: Services
     * #3: Appointments
     */
    public class ShoppingCartViewModel
    {
        public List<Products> Products { get; set; }
        public List<UserServices> UserServices { get; set; }
        public Appointments Appointments { get; set; }
    }
}
