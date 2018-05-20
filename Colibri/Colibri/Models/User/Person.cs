using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.User
{
    // Main Class Person
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string careOf { get; set; }
        public int HouseNumber { get; set; }
        public string City { get; set; }
        public int Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        // default Constuctor
        public Person(int Id, string FirstName, string MiddleName, string LastName, string Street, string careOf, int HouseNumber, string City, int Zip, string State, string Country)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Street = Street;
            this.careOf = careOf;
            this.HouseNumber = HouseNumber;
            this.City = City;
            this.Zip = Zip;
            this.State = State;
            this.Country = Country;
        }
    }
}
