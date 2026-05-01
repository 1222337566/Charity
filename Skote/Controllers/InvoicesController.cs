using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class InvoicesController : Controller
    {
        // GET: Invoices
        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}