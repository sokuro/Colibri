using System;
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
     * Controller to handle all User Appointments
     * 
     * Authorize only the SuperAdmin
     */
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // PageSize (for the Pagination: 5 Appointments/Page)
        private int PageSize = 10;

        public AppointmentsController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        // extend the Method with the Parameters for Search:
        // Name, Email, Phone, Date
        [Route("Admin/Appointments/Index")]
        public async Task<IActionResult> Index(
            int productPage = 1,
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
            // (Pagination Number remains the same)
            StringBuilder param = new StringBuilder();
            // set the default url for the Pagination
            // append search Properties
            param.Append("/Admin/Appointments/Index?productPage=:");
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

            // Pagination
            // count Appointments alltogether
            var count = appointmentViewModel.Appointments.Count;

            // Iterate and Filter Appointments
            // fetch the right Record (if on the 2nd Page, skip the first 3 (if PageSize=3) and continue on the next Page)
            appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                    .OrderBy(p => p.AppointmentDate)
                                                    .Skip((productPage - 1) * PageSize)
                                                    .Take(PageSize).ToList();

            // populate the PagingInfo Model
            appointmentViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            // return the View Model for the Appointments
            return View(appointmentViewModel);
        }

        // Get: Method Edit Appointment
        [Route("Admin/Appointments/Edit/{id}")]
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
                Appointment = await _colibriDbContext.Appointments.Include(a => a.AppPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                // get the Application User List
                AppPerson = _colibriDbContext.ApplicationUsers.ToList(),
                // get the List of all Products
                Products = productList.ToList()
            };

            // return the ViewModel
            return View(objAppointmentVM);

        }

        // POST: Method Edit Appointment
        [Route("Admin/Appointments/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel appointmentViewModel)
        {
            if (ModelState.IsValid)
            {
                // combine the Appointment Date and Time (inside the Appointment Date)
                appointmentViewModel.Appointment.AppointmentDate = appointmentViewModel.Appointment.AppointmentDate
                                                                        .AddHours(appointmentViewModel.Appointment.AppointmentTime.Hour)
                                                                        .AddMinutes(appointmentViewModel.Appointment.AppointmentTime.Minute);

                // retrieve the Appointment Object from the DB
                var appointmentFromDb = await _colibriDbContext.Appointments
                                            .Where(a => a.Id == appointmentViewModel.Appointment.Id)
                                            .FirstOrDefaultAsync();

                // update individual Properties
                appointmentFromDb.CustomerName = appointmentViewModel.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = appointmentViewModel.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = appointmentViewModel.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = appointmentViewModel.Appointment.AppointmentDate;
                appointmentFromDb.isConfirmed = appointmentViewModel.Appointment.isConfirmed;

                // only the SuperAdmin can change this Information
                if (User.IsInRole(StaticDetails.SuperAdminEndUser))
                {
                    appointmentFromDb.AppPersonId = appointmentViewModel.Appointment.AppPersonId;
                }

                // save into the DB
                _colibriDbContext.SaveChanges();

                // redirect
                return RedirectToAction(nameof(Index));
            }

            // return View
            return View(appointmentViewModel);
        }

        // Get: Method Details Appointment
        [Route("Admin/Appointments/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
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
            AppointmentDetailsViewModel appointmentViewModel = new AppointmentDetailsViewModel()
            {
                // retrieve the Appointment from the DB
                Appointment = await _colibriDbContext.Appointments
                                    .Include(a => a.AppPerson)
                                    .Where(a => a.Id == id)
                                    .FirstOrDefaultAsync(),
                // get the Application User List
                AppPerson = _colibriDbContext.ApplicationUsers.ToList(),
                // get the List of all Products
                Products = productList.ToList()
            };

            // return the ViewModel
            return View(appointmentViewModel);
        }

        // Get: Method Delete Appointment
        [Route("Admin/Appointments/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
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
            AppointmentDetailsViewModel appointmentViewModel = new AppointmentDetailsViewModel()
            {
                // retrieve the Appointment from the DB
                Appointment = await _colibriDbContext.Appointments
                                    .Include(a => a.AppPerson)
                                    .Where(a => a.Id == id)
                                    .FirstOrDefaultAsync(),
                // get the Application User List
                AppPerson = _colibriDbContext.ApplicationUsers.ToList(),
                // get the List of all Products
                Products = productList.ToList()
            };

            // return the ViewModel
            return View(appointmentViewModel);
        }

        // POST: Delete Action Method
        [Route("Admin/Appointments/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // get an Appointment from the DB
            var appointment = await _colibriDbContext.Appointments.FindAsync(id);

            _colibriDbContext.Appointments.Remove(appointment);

            await _colibriDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}