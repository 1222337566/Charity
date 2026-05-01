using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class TasksController : Controller
    {
        // GET: Tasks
        public IActionResult TaskCreate()
        {
            return View();
        }

        public IActionResult TaskKanban()
        {
            return View();
        }

        public IActionResult TaskList()
        {
            return View();
        }

    }
}