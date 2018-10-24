﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Extensions
{
    // Extension Method to manipulate Methods
    // Convert List -> IEnumerable<Item>
    public static class IEnumerableExtensions
    {
        // return Value as IEnumerable (Rendering)
        // e.g. ProductTypes
        // 1st Argument of the ExtensionMethod = 'this' Keyword
        // 2nd Argument for DropDown Integer
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            // Linq used
            return from item in items
                   select new SelectListItem
                   {
                       // to retrieve the Name Property from the Collection -> another ExtMethod: 'ReflectionExtension'
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString()),
                   };
        }
    }
}