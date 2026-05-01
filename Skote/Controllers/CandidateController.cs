using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class CandidateController : Controller
    {
        // GET: Candidate
        public IActionResult list()
        {
            return View();
        }
        public IActionResult Overview()
        {
            return View();
        }
    }
}