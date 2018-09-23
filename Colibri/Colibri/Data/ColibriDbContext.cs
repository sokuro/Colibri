using Colibri.Models;
using Colibri.Models.Category;
using Colibri.Models.Category.AudioVideo;
using Colibri.Models.Category.Transport;
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
        public DbSet<Transport> Transports { get; set; }
    }
}
