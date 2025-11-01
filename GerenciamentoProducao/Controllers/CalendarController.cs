using GerenciamentoProducao.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducaoo.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly GoogleCalendarService _calendarService;
        private readonly string _calendarId;
        public CalendarController(GoogleCalendarService calendarService, IConfiguration configuration)
        {
            _calendarService = calendarService;
            _calendarId = configuration["Google:key"];


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
                var calendarId = _calendarId;
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