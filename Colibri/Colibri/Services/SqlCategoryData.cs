using System.Collections.Generic;
using System.Linq;
using Colibri.Data;
using Colibri.Models.Category;

namespace Colibri.Services
{
    public class SqlCategoryData : ICategoryData
    {
        private ColibriDbContext _context;

        public SqlCategoryData(ColibriDbContext context)
        {
            _context = context;
        }


        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.OrderBy(c => c.Id);
        }

        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }
        public Category Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }
    }
}
