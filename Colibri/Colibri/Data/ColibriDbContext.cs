using Colibri.Models;
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
        public ColibriDbContext(DbContextOptions<ColibriDbContext> options) : base(options)
        {

        }


        // Each Entity will need DbSet<T> Property
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }


        // override
        // specify the mapping between the Entities in the DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// filling simple Data
            //modelBuilder.Entity<Order>()
            //    .HasData(
            //    new Order()
            //    {
            //        OrderId = 1,
            //        OrderDate = DateTime.UtcNow,
            //        OrderNumber = "12345"
            //    });
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
              .Property(p => p.UnitPrice)
              .HasColumnType("decimal(18,2)");
        }
    }
}
