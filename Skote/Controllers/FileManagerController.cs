using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class FileManagerController : Controller
    {
        // GET: FileManager
        public IActionResult Index()
        {
            return View();
        }
    }
}