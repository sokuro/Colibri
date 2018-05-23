using Colibri.Models.Category.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    public interface ITransportData
    {
        // Method to get all Transports
        IEnumerable<Transport> GetAll();
        // Method to get a specific Transport by Id
        Transport getById(int id);
        // Method to add a new Transport
        Transport Add(Transport transport);
    }
}
