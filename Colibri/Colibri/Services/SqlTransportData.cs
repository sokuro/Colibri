using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models.Category.Transport;

namespace Colibri.Services
{
    public class SqlTransportData : ITransportData
    {
        private ColibriDbContext _context;

        public SqlTransportData(ColibriDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Transport> GetAll()
        {
            return _context.Transports.OrderBy(t => t.Id);
        }

        public Transport getById(int id)
        {
            return _context.Transports.FirstOrDefault(t => t.Id == id);
        }
        public Transport Add(Transport transport)
        {
            _context.Transports.Add(transport);
            _context.SaveChanges();
            return transport;
        }
    }
}
