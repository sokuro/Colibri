using System.Collections.Generic;
using System.Linq;
using Colibri.Data;
using Colibri.Models;

namespace Colibri.Services
{
    public class SqlCategoryTypesData : ICategoryTypesData
    {
        private ColibriDbContext _context;

        public SqlCategoryTypesData(ColibriDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CategoryTypes> GetAll()
        {
            return _context.CategoryTypes.OrderBy(c => c.Id);
        }

        public CategoryTypes GetById(int id)
        {
            return _context.CategoryTypes.FirstOrDefault(c => c.Id == id);
        }

        public CategoryTypes Add(CategoryTypes categoryTypes)
        {
            _context.CategoryTypes.Add(categoryTypes);
            _context.SaveChanges();
            return categoryTypes;
        }

        public CategoryTypes Remove(CategoryTypes categoryTypes)
        {
            throw new System.NotImplementedException();
        }
    }
}
