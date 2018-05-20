using Colibri.Models.Category.Transport;
using Colibri.Models.User;

namespace Colibri
{
    public class Auto : Vehicle
    {
        // One-to-One Relationships
        public User UserId { get; set; }

        public Auto(int Id, string Brand, string Model, int ConstructionYear, double CargoVolume, double FuelCapacity, double FuelConsumption, string FuelType, string Color, double Price) : base(Id, Brand, Model, ConstructionYear, CargoVolume, FuelCapacity, FuelConsumption, FuelType, Color, Price)
        {
        }
    }
}
