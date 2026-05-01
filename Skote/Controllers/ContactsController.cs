using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ContactsController : Controller
    {
        // GET: Contacts
        public IActionResult ContactsGrid()
        {
            return View();
        }

        public IActionResult ContactsList()
        {
            return View();
        }

        public IActionResult ContactsProfile()
        {
            return View();
        }

    }
}