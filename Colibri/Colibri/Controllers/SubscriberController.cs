﻿using System;
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
        public SubscriberViewModel SubscriberViewModel { get; set; }

        public SubscriberController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;

            SubscriberViewModel = new SubscriberViewModel
            {
                //MyMessage = new List<string>()
                //Notifications = new List<Notifications>()

                Notifications = new Notifications()
            };
        }

        [Route("Subscriber/Index")]
        public async Task<IActionResult> Index()
        {
            //Notifications message = new Notifications();

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.SubscribeAsync<CategoryGroups>("create_category_groups",
                    groups => Task.Factory.StartNew(() =>
                    {
                        var message = "Added Category Group: " + groups.Name;
                        SubscriberViewModel.Notifications.Message = message;
                        SubscriberViewModel.Notifications.NotificationType = "CategoryGroup";
                    }));


                bus.Subscribe<CategoryTypes>("create_category_types", 
                    categoryTypes => Task.Factory.StartNew(() =>
                    {
                        var message = "Added Category Type: " + categoryTypes.Name;
                        SubscriberViewModel.Notifications.Message = message;
                        SubscriberViewModel.Notifications.NotificationType = "CategoryType";
                    }));
            }

            // persist
            await _colibriDbContext.AddAsync(SubscriberViewModel.Notifications);
            await _colibriDbContext.SaveChangesAsync();

            return View(SubscriberViewModel);
        }
    }
}