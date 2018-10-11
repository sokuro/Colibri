using Colibri.Models;
using Colibri.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    public interface ICategoryData
    {
        // Method to get all Categories
        IEnumerable<Categories> GetAll();

        // Method to get a specific Category by ID
        Categories GetById(int id);

        // Method to add a new Category
        Categories Add(Categories categories);
    }
}
