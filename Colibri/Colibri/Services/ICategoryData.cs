using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    public interface ICategoryData
    {
        IEnumerable<Category> GetAll();
    }
}
