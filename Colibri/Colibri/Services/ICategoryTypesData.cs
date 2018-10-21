using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    public interface ICategoryTypesData
    {
        // Method to get all Categories
        IEnumerable<CategoryTypes> GetAll();

        // Method to get a specific Category by ID
        CategoryTypes GetById(int id);

        // Method to add a new Category
        CategoryTypes Add(CategoryTypes categoryTypes);

        CategoryTypes Remove(CategoryTypes categoryTypes);
    }
}
