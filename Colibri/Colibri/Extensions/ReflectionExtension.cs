using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Extensions
{
    // Helper Extension Class to retrieve the Name Property from the Collection
    public static class ReflectionExtension
    {
        // string return Type
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            // retrieve the Property Name and return it as String
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
