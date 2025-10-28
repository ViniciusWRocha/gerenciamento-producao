using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducaoo.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
