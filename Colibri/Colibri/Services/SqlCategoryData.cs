using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.OrderBy(c => c.Id);
        }

        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }
    }
}
