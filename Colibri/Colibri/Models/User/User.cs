using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.User
{
    // Class inherites from the Main Person Class
    class User : Person
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }


        public User(int Id, string FirstName, string MiddleName, string LastName, string Street, string CareOf, int HouseNumber, string City, int Zip, string State, string Country) : base(Id, FirstName, MiddleName, LastName, Street, CareOf, HouseNumber, City, Zip, State, Country)
        {
        }

        
    }
}
