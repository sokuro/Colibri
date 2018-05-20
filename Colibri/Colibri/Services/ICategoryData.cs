using Colibri.Models;
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
        IEnumerable<Category> GetAll();

        // Method to get a specific Category by ID
        Category GetById(int id);
    }
}
