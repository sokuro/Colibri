using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Extensions;
using Colibri.Models;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colibri.Areas.Customer.Controllers
{
    /*
     * Controller of the ordered Items in the Shopping Cart
     */
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ColibriDbContext _colibriDbContext;

        // bind the ShoppingCartViewModel
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public ShoppingCartController(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;

            // initialize the ShoppingCartViewModel
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        // Remove (from Bag)
        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    // remove the Item (id)
                    lstCartItems.Remove(id);
                }
            }
            // set the Session: Name, Value
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            // redirect to Action
            return RedirectToAction(nameof(Index));
        }

        // Get Index ShoppingCart
        // retrieve all the Products from the Session
        public async Task<IActionResult> Index()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssShoppingCart"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                foreach (int cartItem in lstCartItems)
                {
                    // get the Products from the DB
                    // use the eager Method
                    Products products = _colibriDbContext.Products
                        .Include(p => p.CategoryTypes)
                        .Include(p => p.SpecialTags)
                        .Where(p => p.Id == cartItem)
                        .FirstOrDefault();

                    // add the Products to the Shopping Cart
                    ShoppingCartViewModel.Products.Add(products);
                }
            }
            
            // pass the ShoppingCartViewModel to the View
            return View(ShoppingCartViewModel);
        }

        // POST: Index
        // create an Appointment
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            // check first, if anything exists in the Session
            // Session Name : "ssShoppingCart"
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            // merge (add) the Appointment Date and Time to the Appointment Date itself
            ShoppingCartViewModel.Appointments.AppointmentDate = ShoppingCartViewModel.Appointments.AppointmentDate
                                                                    .AddHours(ShoppingCartViewModel.Appointments.AppointmentTime.Hour)
                                                                    .AddMinutes (ShoppingCartViewModel.Appointments.AppointmentTime.Minute);

            // create an Object for the Appointments
            Appointments appointments = ShoppingCartViewModel.Appointments;

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
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            // redirect to Action:
            // ActionMethod: AppointmentConfirmation
            // Controller: ShoppingCart
            // pass the Appointment ID
            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });
        }

        // Get
        // Apointment Confirmation
        public IActionResult AppointmentConfirmation(int id)
        {
            // fill the ViewModel with the Information bound to the specific Id
            ShoppingCartViewModel.Appointments = _colibriDbContext.Appointments
                                                            .Where(a => a.Id == id)
                                                            .FirstOrDefault();

            // based on the Id, retrieve the complete List of Appointments
            List<ProductsSelectedForAppointment> elemProdList = _colibriDbContext.ProductsSelectedForAppointment
                .Where(p => p.AppointmentId == id).ToList();

            // iterate the List
            foreach (ProductsSelectedForAppointment prodObj in elemProdList)
            {
                // add Products inside the Shopping Cart Model
                ShoppingCartViewModel.Products.Add(_colibriDbContext.Products
                                                    .Include(p => p.CategoryTypes)
                                                    .Include(p => p.SpecialTags)
                                                    .Where(p => p.Id == prodObj.ProductId)
                                                    .FirstOrDefault());
            }

            // pass the Shopping Cart View Model as Object
            return View(ShoppingCartViewModel);
        }
    }
}