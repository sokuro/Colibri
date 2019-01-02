using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Colibri.Models;
using Colibri.Utility;
using Colibri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Colibri.Areas.Admin.Controllers
{
    // authorize only the SuperAdminEndUser
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]

    public class SearchesController : Controller
    {
        private ColibriDbContext _colibriDbContext;
        private readonly IStringLocalizer<SearchesController> _localizer;

        // bind to the Archive ViewModel
        // not necessary to create new Objects
        // allowed to use the ViewModel without passing it as ActionMethod Parameter
        [BindProperty]
        public UserSearchesViewModel UserSearchesViewModel { get; set; }

        // Constructor
        public SearchesController(ColibriDbContext colibriDbContext, IStringLocalizer<SearchesController> localizer)
        {
            _colibriDbContext = colibriDbContext;
            _localizer = localizer;

            // ViewModel
            UserSearchesViewModel = new UserSearchesViewModel()
            {
                SearchEntry = new Models.SearchEntry(),

                // Standardmässig keine Einschränkung auf Datum
                dateAll = true,
                date30Days = false,
                dateToday = false,

                // Standardmässig keine Einschränkung auf Ergebnis
                resAll = true,
                resSuccess = false,
                resPartSuccess = false,
                resNoSuccess = false,

                // Standardmässig keine Einschränkunf auf Angebot oder Nachfrage
                searchAll = true,
                searchOffer = false,
                searchRequest = false
            };
        }

        // GET : Action for Index
        [Route("Admin/Searches/Index")]
        public async Task<IActionResult> Index()
        {
            // i18n
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["Export"] = _localizer["ExportText"];
            ViewData["Number"] = _localizer["NumberText"];
            ViewData["Period30"] = _localizer["Period30Text"];
            ViewData["PeriodAll"] = _localizer["PeriodAllText"];
            ViewData["Period"] = _localizer["PeriodText"];
            ViewData["PeriodToday"] = _localizer["PeriodTodayText"];
            ViewData["ResultsAll"] = _localizer["ResultsAllText"];
            ViewData["ResultsNoSuccess"] = _localizer["ResultsNoSuccessText"];
            ViewData["ResultsPartSuccess"] = _localizer["ResultsPartSuccessText"];
            ViewData["ResultsSuccess"] = _localizer["ResultsSuccessText"];
            ViewData["Results"] = _localizer["ResultsText"];
            ViewData["SearchRequests"] = _localizer["SearchRequestsText"];
            ViewData["SearchRequest"] = _localizer["SearchRequestText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["OfferRequest"] = _localizer["OfferRequestText"];
            ViewData["OffReqAll"] = _localizer["OffReqAllText"];
            ViewData["Offer"] = _localizer["OfferText"];
            ViewData["Request"] = _localizer["RequestText"];
            ViewData["DeleteRequests"] = _localizer["DeleteRequestsText"];

            // Alle Einträge
            UserSearchesViewModel.SearchEntryList = await _colibriDbContext.SearchEntry.OrderBy(m => m.SearchText).ToListAsync();

            // Zähler für Ergebnisse aktualisieren
            UserSearchesViewModel.ResultsCounter = UserSearchesViewModel.SearchEntryList.Count();

            return View(UserSearchesViewModel);
        }

        // POST : Action for Index
        [Route("Admin/Searches/Index")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserSearchesViewModel model)
        {
            // i18n
            ViewData["CreatedOn"] = _localizer["CreatedOnText"];
            ViewData["Export"] = _localizer["ExportText"];
            ViewData["Number"] = _localizer["NumberText"];
            ViewData["Period30"] = _localizer["Period30Text"];
            ViewData["PeriodAll"] = _localizer["PeriodAllText"];
            ViewData["Period"] = _localizer["PeriodText"];
            ViewData["PeriodToday"] = _localizer["PeriodTodayText"];
            ViewData["ResultsAll"] = _localizer["ResultsAllText"];
            ViewData["ResultsNoSuccess"] = _localizer["ResultsNoSuccessText"];
            ViewData["ResultsPartSuccess"] = _localizer["ResultsPartSuccessText"];
            ViewData["ResultsSuccess"] = _localizer["ResultsSuccessText"];
            ViewData["Results"] = _localizer["ResultsText"];
            ViewData["SearchRequests"] = _localizer["SearchRequestsText"];
            ViewData["SearchRequest"] = _localizer["SearchRequestText"];
            ViewData["Update"] = _localizer["UpdateText"];
            ViewData["OfferRequest"] = _localizer["OfferRequestText"];
            ViewData["OffReqAll"] = _localizer["OffReqAllText"];
            ViewData["Offer"] = _localizer["OfferText"];
            ViewData["Request"] = _localizer["RequestText"];
            ViewData["DeleteRequests"] = _localizer["DeleteRequestsText"];

            UserSearchesViewModel.resAll = model.resAll;
            UserSearchesViewModel.resSuccess = model.resSuccess;
            UserSearchesViewModel.resPartSuccess = model.resPartSuccess;
            UserSearchesViewModel.resNoSuccess = model.resNoSuccess;
            UserSearchesViewModel.dateAll = model.dateAll;
            UserSearchesViewModel.date30Days = model.date30Days;
            UserSearchesViewModel.dateToday = model.dateToday;
            UserSearchesViewModel.searchAll = model.searchAll;
            UserSearchesViewModel.searchOffer = model.searchOffer;
            UserSearchesViewModel.searchRequest = model.searchRequest;

            UserSearchesViewModel.SearchEntryList = await _colibriDbContext.SearchEntry.OrderBy(m => m.SearchText).ToListAsync();

            // Prüfen, ob Resultate eingeschränkt sind
            if(UserSearchesViewModel.resAll)
            {
                // do nothing?
            }

            if(UserSearchesViewModel.resSuccess)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.FullSuccess == true);
            }

            if (UserSearchesViewModel.resPartSuccess)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.PartSuccess == true);
            }

            if (UserSearchesViewModel.resNoSuccess)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.NoSuccess == true);
            }

            // Prüfen, ob Zeitraum eingeschränkt ist
            if (UserSearchesViewModel.dateAll)
            {
                // do nothing?
            }

            if (UserSearchesViewModel.date30Days)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.SearchDate > (DateTime.Today.AddDays(-1)));
            }

            if (UserSearchesViewModel.dateToday)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.SearchDate.Day == DateTime.Today.Day);
            }

            // Prüfen, ob Angebot oder Nachfrage eingeschränkt ist
            if(UserSearchesViewModel.searchAll)
            {
                // do nothing?
            }

            if(UserSearchesViewModel.searchOffer)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.SearchOffer == true);
            }

            if(UserSearchesViewModel.searchRequest)
            {
                UserSearchesViewModel.SearchEntryList = UserSearchesViewModel.SearchEntryList.Where(m => m.SearchOffer == false);
            }

            // Zähler für Ergebnisse aktualisieren
            UserSearchesViewModel.ResultsCounter = UserSearchesViewModel.SearchEntryList.Count();

            return View(UserSearchesViewModel);
        }

        // GET : Action for Delete
        [Route("Admin/Searches/Delete")]
        public async Task<IActionResult> Delete()
        {
            // i18n
            ViewData["ConfirmDelete"] = _localizer["ConfirmDeleteText"];
            ViewData["Cancel"] = _localizer["CancelText"];
            ViewData["Delete"] = _localizer["DeleteText"];
            ViewData["DeleteRequests"] = _localizer["DeleteRequestsText"];
            ViewData["Number"] = _localizer["NumberText"];

            UserSearchesViewModel.ResultsCounter = _colibriDbContext.SearchEntry.Count();

            return View(UserSearchesViewModel);
        }

        // POST : Action for Delete
        [Route("Admin/Searches/Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST()
        {
            // Suchanfragen von DB löschen
            var entries = _colibriDbContext.SearchEntry.ToList();

            foreach(var x in entries)
            {
                _colibriDbContext.Remove(x);
            }
            await _colibriDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}