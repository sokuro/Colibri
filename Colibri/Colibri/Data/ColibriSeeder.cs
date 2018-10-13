using Colibri.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public ColibriSeeder(ColibriDbContext context, IHostingEnvironment hosting, UserManager<User> userManager)
        {
            _context = context;
            _hosting = hosting;
            _userManager = userManager;
        }

        // Method to seed Data
        public async Task Seed()
        {
            _context.Database.EnsureCreated();

            // add User for Authorization
            // await for Synchronizing
            User user = await _userManager.FindByEmailAsync("john.snow.js96@protonmail.com");

            // avoiding NullPointerException, create a new User
            if (user == null)
            {
                user = new User()
                {
                    FirstName = "John",
                    LastName = "Snow",
                    Email = "john.snow.js96@protonmail.com",
                    UserName = "john.snow.js96@protonmail.com"
                };

                // use the UserManager to create this new User
                var result = await _userManager.CreateAsync(user, "P@ssw0rd!");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create a new User in Seeder");
                }
            }

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
