using Colibri.Models;
using Colibri.Models.Category;
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
                new Categories { Id = 1, Name = "Transport" },
                new Categories { Id = 2, Name = "Audio, TV, Video, Foto" },
                new Categories { Id = 3, Name = "Haushalt" },
                new Categories { Id = 4, Name = "Garten" },
                new Categories { Id = 5, Name = "Musik" },
                new Categories { Id = 6, Name = "Sport" }
            };
        }


        public IEnumerable<Categories> GetAll()
        {
            return _categories.OrderBy(c => c.Id);
        }
        public Categories GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }
        public Categories Add(Categories category)
        {
            category.Id = _categories.Max(c => c.Id) + 1;
            _categories.Add(category);
            return category;
        }
    }
}
