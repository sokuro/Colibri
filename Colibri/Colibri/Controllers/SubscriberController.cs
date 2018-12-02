using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Colibri.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        [BindProperty]
        public SubscriberViewModel SubscriberViewModel { get; }

        //public SubscriberController(ColibriDbContext colibriDbContext)
        //{
        //    _colibriDbContext = colibriDbContext;

        //    SubscriberViewModel = new SubscriberViewModel {
        //        MyMessage = new List<string>()
        //        //Notifications = new List<Notifications>()
        //    };
        //}
        public SubscriberController()
        {
            SubscriberViewModel = new SubscriberViewModel
            {
                //MyMessage = new List<string>()
                Notifications = new Notifications()
            };
        }

        [Route("Subscriber/Index")]
        public IActionResult Index()
        {
            //Notifications message = new Notifications();

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<CategoryGroups>("create_category_groups", HandleAddedCategoryGroupMessage);
            }

            void HandleAddedCategoryGroupMessage(CategoryGroups obj1)
            {
                //Console.WriteLine("Added Category Name: {0}", obj1.Name);
                //message = "Added Category Name: {0}" + obj1.Name;
                //ViewBag.MyMessagesList = "Added Category Name: {0}" + obj1.Name;

                var message = "Added Category Name: " + obj1.Name;

                //SubscriberViewModel.Notifications.Add(message);
                //_colibriDbContext.Add(SubscriberViewModel.Notifications);

                SubscriberViewModel.Notifications.Message = message;


                //_colibriDbContext.SaveChanges();

            }

            //return View("Index", message);
            return View(SubscriberViewModel);
            //return View();
        }
    }
}