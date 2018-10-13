using System.Collections.Generic;
using System.Linq;
using Colibri.Data;
using Colibri.Models;

namespace Colibri.Services
{
    public class SqlCategoryData : ICategoryData
    {
        private ColibriDbContext _context;

        public SqlCategoryData(ColibriDbContext context)
        {
            _context = context;
        }


        public IEnumerable<Categories> GetAll()
        {
            return _context.Categories.OrderBy(c => c.Id);
        }

        public Categories GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }
        public Categories Add(Categories category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }
    }
}
