using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category.Transport
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ConstructionYear { get; set; }
        public int CargoVolume { get; set; }
        public int FuelCapacity { get; set; }
        public double FuelConsumption { get; set; }
        public string FuelType { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
    }
}
