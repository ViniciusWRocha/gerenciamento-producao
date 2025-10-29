using GerenciamentoProducao.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducaoo.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly GoogleCalendarService _calendarService;

        public CalendarController(GoogleCalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            try
            {
                var calendarId = "e96a4fe0acce51e1436e1b25ecfd9055123036df7caabdbfcad011b2a82111fb@group.calendar.google.com";
                var events = _calendarService.ListEvents(calendarId);

                var formattedEvents = events.Select(ev => new
                {
                    title = ev.Summary,
                    start = ev.Start.DateTimeRaw ?? ev.Start.Date,
                    end = ev.End.DateTimeRaw ?? ev.End.Date,
                    description = ev.Description,
                    htmlLink = ev.HtmlLink
                }).ToList();

                return Json(formattedEvents);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}