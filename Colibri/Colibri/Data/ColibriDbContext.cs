using Colibri.Models;
using Microsoft.AspNetCore.Identity;
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
    //public class ColibriDbContext : IdentityDbContext<ApplicationUser>
    public class ColibriDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ColibriDbContext(DbContextOptions<ColibriDbContext> options) : base(options)
        {

        }


        // Each Entity will need DbSet<T> Property
        public DbSet<CategoryGroups> CategoryGroups { get; set; }
        public DbSet<CategoryTypes> CategoryTypes { get; set; }
        public DbSet<SpecialTags> SpecialTags { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<UserServices> UserServices { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointment { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Notifications> Notifications { get; set; }


        // override
        // specify the mapping between the Entities in the DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
