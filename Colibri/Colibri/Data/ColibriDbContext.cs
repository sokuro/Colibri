using Colibri.Models;
using Colibri.Models.Category;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    // ColibriContext derived from the IdentityDbContext
    public class ColibriDbContext : IdentityDbContext<User>
    {
        public ColibriDbContext(DbContextOptions options) : base(options)
        {

        }

        // Each Entity will need DbSet<T> Property
        public DbSet<Category> Categories { get; set; }
    }
}
