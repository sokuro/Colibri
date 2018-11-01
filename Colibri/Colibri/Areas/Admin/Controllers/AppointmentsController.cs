﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Colibri.Data;
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

        public async Task<IActionResult> Index()
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

            // return the View Model for the Appointments
            return View(appointmentViewModel);
        }
    }
}