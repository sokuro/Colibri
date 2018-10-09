using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    // Db Seeder
    public class ColibriSeeder
    {
        private readonly ColibriDbContext _context;

        public ColibriSeeder(ColibriDbContext context)
        {
            _context = context;
        }

        // Method to seed Data
        public void Seed()
        {
            _context.Database.EnsureCreated();
        }
    }
}
