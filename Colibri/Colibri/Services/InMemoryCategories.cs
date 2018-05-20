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
        private List<Category> _categories;

        // Initialize the List
        // Constructor
        public InMemoryCategories()
        {
            // create some new Categories
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Transport" },
                new Category { Id = 2, Name = "Audio, TV, Video, Foto" },
                new Category { Id = 3, Name = "Haushalt" },
                new Category { Id = 4, Name = "Garten" },
                new Category { Id = 5, Name = "Musik" },
                new Category { Id = 6, Name = "Sport" }
            };
        }


        public IEnumerable<Category> GetAll()
        {
            return _categories.OrderBy(c => c.Id);
        }
        public Category GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }
        public Category Add(Category category)
        {
            category.Id = _categories.Max(c => c.Id) + 1;
            _categories.Add(category);
            return category;
        }
    }
}
