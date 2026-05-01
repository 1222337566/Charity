using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public IActionResult Index()
        {
            return View();
        }
    }
}