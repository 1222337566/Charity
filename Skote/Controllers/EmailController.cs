using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public IActionResult EmailInbox()
        {
            return View();
        }

        public IActionResult EmailRead()
        {
            return View();
        }

        public IActionResult EmailTemplateAlert()
        {
            return View();
        }

        public IActionResult EmailTemplateBasic()
        {
            return View();
        }

        public IActionResult EmailTemplateBilling()
        {
            return View();
        }

    }
}