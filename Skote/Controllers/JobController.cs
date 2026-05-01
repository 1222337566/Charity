using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class JobController : Controller
    {
        // GET: Job
        public IActionResult apply()
        {
            return View();
        }
        public IActionResult categories()
        {
            return View();
        }
        public IActionResult details()
        {
            return View();
        }
        public IActionResult grid()
        {
            return View();
        }
        public IActionResult list()
        {
            return View();
        }
    }
}