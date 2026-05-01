using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index1()
        {
            return View();
        }
        public IActionResult CalendarFull()
        {
            return View();
        }
    }
}