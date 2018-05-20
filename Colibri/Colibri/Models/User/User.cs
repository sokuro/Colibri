using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.User
{
    // Class inherites from the Main Person Class
    public class User : Person
    {
        public User(int Id, string FirstName, string MiddleName, string LastName, string Street, string careOf, int HouseNumber, string City, int Zip, string State, string Country) : base(Id, FirstName, MiddleName, LastName, Street, careOf, HouseNumber, City, Zip, State, Country)
        {
        }
    }
}
