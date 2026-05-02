using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Grid()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}