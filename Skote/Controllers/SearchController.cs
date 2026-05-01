using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        public IActionResult Index(string? q)
        {
            ViewBag.Query = q ?? "";
            return View();
        }
    }
}
