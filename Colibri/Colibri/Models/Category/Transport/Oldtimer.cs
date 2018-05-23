using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category.Transport
{
    public class Oldtimer : Vehicle
    {
        public Oldtimer(int Id, string Brand, string Model, int ConstructionYear, double CargoVolume, double FuelCapacity, double FuelConsumption, string FuelType, string Color, double Price) : base(Id, Brand, Model, ConstructionYear, CargoVolume, FuelCapacity, FuelConsumption, FuelType, Color, Price)
        {
        }
    }
}
