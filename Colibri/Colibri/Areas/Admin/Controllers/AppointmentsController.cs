﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Areas.Admin.Controllers
{
    /*
     * Controller to handle the User Appointments
     * 
     * Authorize both the Admin and SuperAdmin
     */
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        public AppointmentsController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        // extend the Method with the Parameters for Search:
        // Name, Email, Phone, Date
        public async Task<IActionResult> Index(
            string searchName=null,
            string searchEmail=null,
            string searchPhone=null,
            string searchDate=null)
        {
            // Security Claims
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // Claims Identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // only if the User is AdminUser -> get ID
            AppointmentViewModel appointmentViewModel = new AppointmentViewModel
            {
                // initialize
                Appointments = new List<Models.Appointments>()
            };

            // Filter the Search Criteria
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }


            // include the Appointments Person (populate it)
            appointmentViewModel.Appointments = _colibriDbContext.Appointments
                                                    .Include(a => a.AppPerson).ToList();

            // check if the User is Admin or SuperAdmin
            if (User.IsInRole(StaticDetails.AdminEndUser))
            {
                // AdminEndUser sees only Appoints addressed to them!!!
                appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                        .Where(a => a.AppPersonId == claim.Value)
                                                        .ToList();
            }

            // Search Conditions
            if (searchName != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentViewModel.Appointments = appointmentViewModel.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }

            }

            // return the View Model for the Appointments
            return View(appointmentViewModel);
        }

        // Get: Method Edit Appointment
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // retrieve IEnumerable of Products as List
            // JOIN Tables
            var productList = (IEnumerable<Products>)(from p in _colibriDbContext.Products
                                                      join a in _colibriDbContext.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("CategoryTypes");

            // use the ViewModel
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                // retrieve the Appointment from the DB
                Appointment = _colibriDbContext.Appointments.Include(a => a.AppPerson).Where(a => a.Id == id).FirstOrDefault(),
                // get the Application User List
                AppPerson = _colibriDbContext.ApplicationUsers.ToList(),
                // get the List of all Products
                Products = productList.ToList()
            };

            // return the ViewModel
            return View(objAppointmentVM);

        }
    }
}