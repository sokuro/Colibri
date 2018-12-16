using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.Services;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller of the ordered Items in the Scheduling
     */
    [Area("Customer")]
    public class SchedulingController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<SchedulingController> _localizer;

        // bind the SchedulingViewModel
        [BindProperty]
        public SchedulingViewModel SchedulingViewModel { get; set; }

        public SchedulingController(ColibriDbContext colibriDbContext, 
            IEmailSender emailSender,
            IStringLocalizer<SchedulingController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _emailSender = emailSender;
            _localizer = localizer;

            // initialize the SchedulingViewModel
            SchedulingViewModel = new SchedulingViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        // Get Index Scheduling
        // retrieve all the Products from the Session
        [Route("Customer/Scheduling/Index")]
        public async Task<IActionResult> Index()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssScheduling"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssScheduling");

            if (lstCartItems != null && lstCartItems.Any())
            {
                foreach (int cartItem in lstCartItems)
                {
                    // get the Products from the DB
                    // use the eager Method
                    Products products = await _colibriDbContext.Products
                        .Include(p => p.CategoryTypes)
                        //.Include(p => p.SpecialTags)
                        .Where(p => p.Id == cartItem)
                        .FirstOrDefaultAsync();

                    // add the Products to the Scheduling
                    SchedulingViewModel.Products.Add(products);
                }
            }

            // i18n
            ViewData["SchedulingTitle"] = _localizer["SchedulingTitleText"];
            ViewData["ScheduledItemName"] = _localizer["ScheduledItemNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["NameSchedulingUser"] = _localizer["NameSchedulingUserText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberTextText"];
            ViewData["Email"] = _localizer["EmailText"];
            ViewData["AppointmentDate"] = _localizer["AppointmentDateText"];
            ViewData["AppointmentTime"] = _localizer["AppointmentTimeText"];
            ViewData["ScheduleAppointment"] = _localizer["ScheduleAppointmentText"];
            ViewData["NoItemsAdded"] = _localizer["NoItemsAddedText"];

            // pass the SchedulingViewModel to the View
            return View(SchedulingViewModel);
        }

        // POST: Index
        // create an Appointment
        [Route("Customer/Scheduling/Index")]
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssScheduling"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssScheduling");

            // merge (add) the Appointment Date and Time to the Appointment Date itself
            SchedulingViewModel.Appointments.AppointmentDate = SchedulingViewModel.Appointments.AppointmentDate
                                                                    .AddHours(SchedulingViewModel.Appointments.AppointmentTime.Hour)
                                                                    .AddMinutes (SchedulingViewModel.Appointments.AppointmentTime.Minute);

            // create an Object for the Appointments
            Appointments appointments = SchedulingViewModel.Appointments;

            // add the Appointments to the DB
            _colibriDbContext.Appointments.Add(appointments);
            _colibriDbContext.SaveChanges();

            // by saving one gets the Appointment Id that has been just created
            int appointmentId = appointments.Id;

            // this created Id can be used to insert Records inside the selected Products
            foreach (int productId in lstCartItems)
            {
                // everytime a new Object will be created
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };

                // add to the DB
                _colibriDbContext.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);
            }
            // save the Changes all together after the Iteration
            _colibriDbContext.SaveChanges();

            // After adding the Items to the DB, empty the Cart (by creating a new Session)
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssScheduling", lstCartItems);

            // TODO
            // send Email
            _emailSender.SendEmailAsync(SchedulingViewModel.Appointments.CustomerEmail, "Your Order at Colibri", $"We are happy to inform you about your Order");

            // redirect to Action:
            // ActionMethod: AppointmentConfirmation
            // Controller: Scheduling
            // pass the Appointment ID
            return RedirectToAction("AppointmentConfirmation", "Scheduling", new { Id = appointmentId });
        }

        // Get
        // Apointment Confirmation
        [Route("Customer/Scheduling/AppointmentConfirmation/{id}")]
        public IActionResult AppointmentConfirmation(int id)
        {
            // fill the ViewModel with the Information bound to the specific Id
            SchedulingViewModel.Appointments = _colibriDbContext.Appointments
                                                            .Where(a => a.Id == id)
                                                            .FirstOrDefault();

            // based on the Id, retrieve the complete List of Appointments
            List<ProductsSelectedForAppointment> elemProdList = _colibriDbContext.ProductsSelectedForAppointment
                .Where(p => p.AppointmentId == id).ToList();

            // iterate the List
            foreach (ProductsSelectedForAppointment prodObj in elemProdList)
            {
                // add Products inside the Scheduling Model
                SchedulingViewModel.Products.Add(_colibriDbContext.Products
                                                    .Include(p => p.CategoryTypes)
                                                    //.Include(p => p.SpecialTags)
                                                    .Where(p => p.Id == prodObj.ProductId)
                                                    .FirstOrDefault());
            }

            // pass the Scheduling View Model as Object
            return View(SchedulingViewModel);
        }

        // Remove (from Bag)
        [Route("Customer/Scheduling/Remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssScheduling");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    // remove the Item (id)
                    lstCartItems.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("ssScheduling", lstCartItems);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }
    }
}