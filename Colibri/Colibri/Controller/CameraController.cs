using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Colibri
{
    public class CameraController : Controller
    {
        private ICameraData _cameraData;

        public CameraController(ICameraData cameraData)
        {
            _cameraData = cameraData;
        }
        public IActionResult Index()
        {
            var model = new CameraIndexViewModel();
            model.Cameras = _cameraData.GetAll();

            return View(model);
        }
    }
}