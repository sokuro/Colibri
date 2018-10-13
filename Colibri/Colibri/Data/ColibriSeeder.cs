using Colibri.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    // Db Seeder
    public class ColibriSeeder
    {
        private readonly ColibriDbContext _context;
        private readonly IHostingEnvironment _hosting;

        public ColibriSeeder(ColibriDbContext context, IHostingEnvironment hosting)
        {
            _context = context;
            _hosting = hosting;
        }

        // Method to seed Data
        public void Seed()
        {
            _context.Database.EnsureCreated();

            // check: data in DB?
            if (!_context.Categories.Any())
            {
                // create some sample data
                // get the json data (art.json)
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filepath);

                // add categories
                // use Deserialization
                var categories = JsonConvert.DeserializeObject<IEnumerable<Categories>>(json);
                // addRange to cut the Collection (large!)
                _context.Categories.AddRange(categories);

                // save changes
                _context.SaveChanges();
            }
        }
    }
}
