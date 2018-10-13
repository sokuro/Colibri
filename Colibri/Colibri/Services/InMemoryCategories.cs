using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    // Implement the ICategoryData Interface
    public class InMemoryCategories : ICategoryData
    {
        private List<Categories> _categories;

        // Initialize the List
        // Constructor
        public InMemoryCategories()
        {
            // create some new Categories
            _categories = new List<Categories>
            {
                new Categories { CategoryId = 1, Name = "Transport" },
                new Categories { CategoryId = 2, Name = "Audio, TV, Video, Foto" },
                new Categories { CategoryId = 3, Name = "Haushalt" },
                new Categories { CategoryId = 4, Name = "Garten" },
                new Categories { CategoryId = 5, Name = "Musik" },
                new Categories { CategoryId = 6, Name = "Sport" }
            };
        }


        public IEnumerable<Categories> GetAll()
        {
            return _categories.OrderBy(c => c.CategoryId);
        }
        public Categories GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.CategoryId == id);
        }
        public Categories Add(Categories category)
        {
            category.CategoryId = _categories.Max(c => c.CategoryId) + 1;
            _categories.Add(category);
            return category;
        }
    }
}
