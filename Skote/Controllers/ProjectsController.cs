using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;

namespace Skote.Controllers
{
    [Authorize(Policy = CharityPolicies.ProjectsView)]
    public class ProjectsController : Controller
    {
        [Authorize(Policy = CharityPolicies.ProjectsManage)]
        // GET: Projects
        public IActionResult ProjectCreate()
        {
            return View();
        }

        public IActionResult ProjectGrid()
        {
            return View();
        }

        public IActionResult ProjectList()
        {
            return View();
        }

        public IActionResult ProjectOverview()
        {
            return View();
        }

    }
}