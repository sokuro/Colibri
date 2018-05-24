using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models.Category.AudioVideo;

namespace Colibri.Services
{
    public class SqlCameraData : ICameraData
    {
        private ColibriDbContext _context;

        public SqlCameraData(ColibriDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Camera> GetAll()
        {
            return _context.Cameras.OrderBy(c => c.Id);
        }

        public Camera GetById(int Id)
        {
            return _context.Cameras.FirstOrDefault(c => c.Id == Id);
        }
        public Camera Add(Camera camera)
        {
            _context.Cameras.Add(camera);
            _context.SaveChanges();
            return camera;
        }
    }
}
