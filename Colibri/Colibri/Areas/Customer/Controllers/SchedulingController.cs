using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.Services;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Colibri.Extensions.Encoding;

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
                        .Include(p => p.CategoryGroups)
                        .Include(p => p.CategoryTypes)
                        .Where(p => p.Id == cartItem)
                        .FirstOrDefaultAsync();

                    // add the Products to the Scheduling
                    SchedulingViewModel.Products.Add(products);
                }
            }

            // i18n
            ViewData["Scheduling"] = _localizer["SchedulingText"];
            ViewData["ScheduledItemName"] = _localizer["ScheduledItemNameText"];
            ViewData["Price"] = _localizer["PriceText"];
            ViewData["CategoryGroup"] = _localizer["CategoryGroupText"];
            ViewData["CategoryType"] = _localizer["CategoryTypeText"];
            ViewData["Description"] = _localizer["DescriptionText"];
            ViewData["UserName"] = _localizer["UserNameText"];
            ViewData["NameSchedulingUser"] = _localizer["NameSchedulingUserText"];
            ViewData["PhoneNumber"] = _localizer["PhoneNumberText"];
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

            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // add the current User as the Appointments Customer
            SchedulingViewModel.Appointments.CustomerId = claim.Value;
            // get the Customer's Properties
            SchedulingViewModel.Appointments.Customer = _colibriDbContext.ApplicationUsers
                .FirstOrDefault(u => u.Id == SchedulingViewModel.Appointments.CustomerId);

            SchedulingViewModel.Appointments.CustomerEmail = SchedulingViewModel.Appointments.Customer.Email;
            SchedulingViewModel.Appointments.CustomerPhoneNumber = SchedulingViewModel.Appointments.Customer.PhoneNumber;

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
                // add the Product's Owner
                SchedulingViewModel.Appointments.AppPersonId = _colibriDbContext.Products
                    .FirstOrDefault(p => p.Id == productId).ApplicationUserId;


                // everytime a new Object will be created
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };

                // add to the DB ProductsSelectedForAppointment
                _colibriDbContext.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);
            }
            // save the Changes all together after the Iteration
            _colibriDbContext.SaveChanges();

            // After adding the Items to the DB, empty the Cart (by creating a new Session)
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssScheduling", lstCartItems);

            // TODO
            // send Email: to the Customer and the Owner
            // build a Template mit Customers Details
            //_emailSender.SendEmailAsync(
            //    SchedulingViewModel.Appointments.CustomerEmail, 
            //    "Your Order at Colibri", 
            //    $"We are happy to inform you about your Order:" +
            //    $"OrderNo.: " + SchedulingViewModel.Products.FirstOrDefault().Id);

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
                                                    .Include(p => p.CategoryGroups)
                                                    .Include(p => p.CategoryTypes)
                                                    .Include(p => p.ApplicationUser)
                                                    .Where(p => p.Id == prodObj.ProductId)
                                                    .FirstOrDefault());

                // get the CustomerData for the Product's Owner Data
                SchedulingViewModel.Appointments.Customer = _colibriDbContext.ApplicationUsers
                .FirstOrDefault(u => u.Id == SchedulingViewModel.Appointments.CustomerId);
            }

            // TODO
            // handle Image
            string colibriAppIcon = "~\\img\\SystemImages\\colibri.png";
            string imageSource = System.Text.Encoding.UTF8.EncodeBase64(colibriAppIcon);


            // send Email: to the Customer and the Owner
            // build a Template mit Customers Details
            // <html> Version
            _emailSender.SendEmailAsync(
                SchedulingViewModel.Appointments.CustomerEmail,
                "Your Reservation at Colibri",

                //$"<p><img src='~\\img\\SystemImages\\colibri.png' /></p>" +
                $"<p><img src='" + imageSource + "' /></p>" +


                $"<p>Hello " + SchedulingViewModel.Appointments.Customer.UserName + "</p></br>" +
                $"<p>We are happy to inform you about your Reservation of the following Product:" + "</p>" +
                $"<p><img src='~" + SchedulingViewModel.Products.FirstOrDefault().Image + "' /></p>" +
                $"<p>Name: " + SchedulingViewModel.Products.FirstOrDefault().Name + "</p>" +
                $"<p>on " + SchedulingViewModel.Appointments.AppointmentDate + "</p>" +
                $"<p>at " + SchedulingViewModel.Appointments.AppointmentTime + "</p>" +
                $"<p>Thank you, " + "</p>" +
                $"<p>Your Colibri Team</p>");

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