using Colibri.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    public class ColibriDbContext : DbContext
    {
        public ColibriDbContext(DbContextOptions options) : base(options)
        {

        }

        // Each Entity will need DbSet<T> Property
        public DbSet<Category> Categories { get; set; }
    }
}
