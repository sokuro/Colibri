using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Colibri.Areas.Admin.Controllers;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;

namespace Colibri.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    public class SubscriberController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<SubscriberController> _localizer;

        private int PageSize = 10;

        [BindProperty]
        public SubscriberViewModel SubscriberViewModel { get; set; }

        public SubscriberController(ColibriDbContext colibriDbContext,
            IStringLocalizer<SubscriberController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            SubscriberViewModel = new SubscriberViewModel
            {
                //MyMessage = new List<string>()
                //Notifications = new List<Notifications>()

                Notifications = new Notifications(),
                ApplicationUserCategoryTypesSubscriber = new ApplicationUserCategoryTypesSubscriber(),
                NotificationsEnumerable = new List<Notifications>(),
                CategoryTypes = _colibriDbContext.CategoryTypes.ToList()
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
            //SubscriberViewModel.Notifications.CategoryTypes = product.CategoryTypes;
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

            // i18n
            ViewData["CurrentUser"] = _localizer["CurrentUserText"];
            ViewData["Message"] = _localizer["MessageText"];
            ViewData["NotificationType"] = _localizer["NotificationTypeText"];
            ViewData["CategoryTypeId"] = _localizer["CategoryTypeIdText"];
            ViewData["CategoryTypes"] = _localizer["CategoryTypesText"];

            return View(notificationsList);
        }

        // Show all CategoryTypes for Notifications
        public async Task<IActionResult> ShowAllCategoryTypesForNotifications()
        {
            SubscriberViewModel.CategoryTypes = await _colibriDbContext.CategoryTypes.ToListAsync();

            // i18n
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Name"] = _localizer["NameText"];

            return View(SubscriberViewModel);
        }

        // filter with id
        public async Task<IActionResult> ShowMyNotifications()
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            SubscriberViewModel.CurrentUserId = claim.Value;

            SubscriberViewModel.NotificationsEnumerable = (IEnumerable<Notifications>)(from n in _colibriDbContext.Notifications
                join s in _colibriDbContext.ApplicationUserCategoryTypesSubscribers
                    on n.CategoryTypeId equals s.CategoryTypeId
                select n)
                .Distinct();

            if (SubscriberViewModel.Notifications == null)
            {
                return NotFound();
            }

            // i18n
            ViewData["CurrentUser"] = _localizer["CurrentUserText"];
            ViewData["Message"] = _localizer["MessageText"];
            ViewData["NotificationType"] = _localizer["NotificationTypeText"];
            ViewData["CategoryTypeId"] = _localizer["CategoryTypeIdText"];
            ViewData["CategoryTypes"] = _localizer["CategoryTypesText"];

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