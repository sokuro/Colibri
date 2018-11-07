using Colibri.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.ViewComponents
{
    /*
     * using ViewComponent to display the User's Name instead of the EMail Address
     */
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ColibriDbContext _colibriDbContext;

        public UserNameViewComponent(ColibriDbContext colibriDbContext)
        {
            _colibriDbContext = colibriDbContext;
        }

        // use Claims Identity
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _colibriDbContext.ApplicationUsers
                                        .Where(u => u.Id == claims.Value)
                                        .FirstOrDefaultAsync();

            return View(userFromDb);
        }
    }
}
