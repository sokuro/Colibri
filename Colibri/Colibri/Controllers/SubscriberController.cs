using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                //bus.Subscribe<Products>("create_product_by_admin",
                //    categoryTypes => Task.Factory.StartNew(() =>
                //    {
                //        var message = "Added Product by Admin: " + typeof(Products);
                //        SubscriberViewModel.Notifications.Message = message;
                //        SubscriberViewModel.Notifications.NotificationType = "ProductsByAdmin";
                //    }));
            }

            // persist
            if (SubscriberViewModel.Notifications.Message != null)
            {
                await _colibriDbContext.AddAsync(SubscriberViewModel.Notifications);
                await _colibriDbContext.SaveChangesAsync();
            }

            return View(SubscriberViewModel);
        }

        public async Task<IActionResult> SendReceive()
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Receive("create_product_by_admin", x => x.Add<Products>(p => HandleProductByAdmin(p)));
                bus.Receive("create_advertisement", x => x.Add<Products>(p => HandleAdvertisements(p)));
                //bus.Receive("create_category_groups", x => x.Add<CategoryGroups>(p => HandleCategoryGroups(p)));
                //bus.Receive("create_category_types", x => x.Add<CategoryTypes>(p => HandleCategoryTypes(p)));
            }

            // persist
            if (SubscriberViewModel.Notifications.Message != null)
            {
                await _colibriDbContext.AddAsync(SubscriberViewModel.Notifications);
                await _colibriDbContext.SaveChangesAsync();
            }

            return View(SubscriberViewModel);
        }

        private void HandleAdvertisements(Products product)
        {
            SubscriberViewModel.Notifications.Message = "Added a Advertisement: " + product.Name;
            SubscriberViewModel.Notifications.NotificationType = product.CategoryTypes.Name;
            SubscriberViewModel.Notifications.CategoryTypes = product.CategoryTypes;
            SubscriberViewModel.Notifications.CategoryTypeId = product.CategoryTypes.Id;
        }

        private void HandleCategoryGroups(CategoryGroups categoryGroup)
        {
            SubscriberViewModel.Notifications.Message = "Added a Category Group: " + categoryGroup.Name;
        }

        private void HandleCategoryTypes(CategoryTypes categoryType)
        {
            SubscriberViewModel.Notifications.Message = "Added a Category Type: " + categoryType.Name;
        }

        private void HandleProductByAdmin(Products product)
        {
            SubscriberViewModel.Notifications.Message = "Added a Product by Admin: " + product.Name;
            SubscriberViewModel.Notifications.NotificationType = product.CategoryTypes.Name;

            // set the Session: Name, Value
            HttpContext.Session.Set("productsByAdminNotifications", product);
            //HttpContext.Session.Set("productsByAdminNotifications", SubscriberViewModel.Notifications);
        }

        // show all Notifications
        public async Task<IActionResult> ShowAllNotifications()
        {
            var notificationsList = await _colibriDbContext.Notifications.ToListAsync();

            return View(notificationsList);
        }

        // filter with id
        public async Task<IActionResult> ShowMyNotifications(int? id)
        {
            SubscriberViewModel.Notifications = await _colibriDbContext.Notifications
                .SingleOrDefaultAsync(n => n.Id == id);

            if (SubscriberViewModel.Notifications == null)
            {
                return NotFound();
            }

            return View(SubscriberViewModel);
        }

        // filter with string
        public async Task<IActionResult> ShowMyNotificationsString(string notificationType)
        {
            SubscriberViewModel.Notifications = await _colibriDbContext.Notifications
                .SingleOrDefaultAsync(n => n.NotificationType.Contains(notificationType));

            if (SubscriberViewModel.Notifications == null)
            {
                return NotFound();
            }

            return View(SubscriberViewModel);
        }
    }
}