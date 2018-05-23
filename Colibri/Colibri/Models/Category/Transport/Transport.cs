using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category.Transport
{
    public class Transport
    {
        public int Id { get; set; }
        public DbSet<Auto> Autos { get; set; }
        public DbSet<Caravan> Caravans { get; set; }
        public DbSet<Miscellaneous> Miscellaneous{ get; set; }
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<Oldtimer> Oldtimers { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Tractor> Tractors { get; set; }
        public DbSet<Trailer> Trailers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Vespa> Vespas { get; set; }
    }
}
