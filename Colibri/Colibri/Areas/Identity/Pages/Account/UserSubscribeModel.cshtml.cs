using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Colibri.Areas.Identity.Pages.Account
{
    public class UserSubscribeModel : PageModel
    {
        private readonly ColibriDbContext _colibriDbContext;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public UserSubscribeModel(
            ColibriDbContext colibriDbContext,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _colibriDbContext = colibriDbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty] public UserSubscribeNotificationsIndexViewModel UserSubscribeNotificationsIndexViewModel { get;
            set;
        }

        public async Task<IActionResult> OnGet()
        {
            UserSubscribeNotificationsIndexViewModel = new UserSubscribeNotificationsIndexViewModel()
            {
                UserSubscriber = await _colibriDbContext.ApplicationUserCategoryTypesSubscribers
                    .Include(m => m.CategoryTypes).ToListAsync(),
                CategoryTypes = _colibriDbContext.CategoryTypes.OrderBy(d => d.Name)
            };

            return Page();
        }
    }
}