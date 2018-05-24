using Colibri.Models.Category.AudioVideo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    public interface ICameraData
    {
        IEnumerable<Camera> GetAll();
        Camera GetById(int Id);
        Camera Add(Camera camera);
    }
}
