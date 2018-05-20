using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category.Transport
{
    // Main Class Vehicle
    public class Vehicle
    {
        // Accessors
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ConstructionYear { get; set; }
        public double CargoVolume { get; set; }
        public double FuelCapacity { get; set; }
        public double FuelConsumption { get; set; }
        public string FuelType { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }

        // Default Constructor
        public Vehicle(int Id, string Brand, string Model, int ConstructionYear, double CargoVolume, double FuelCapacity, double FuelConsumption, string FuelType, string Color, double Price)
        {
            this.Id = Id;
            this.Brand = Brand;
            this.Model = Model;
            this.ConstructionYear = ConstructionYear;
            this.CargoVolume = CargoVolume;
            this.FuelCapacity = FuelCapacity;
            this.FuelConsumption = FuelConsumption;
            this.FuelType = FuelType;
            this.Color = Color;
            this.Price = Price;
        }
    }
}
