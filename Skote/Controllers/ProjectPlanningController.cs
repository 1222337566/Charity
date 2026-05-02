using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPlanningController : Controller
    {
        private readonly IProjectPlanningRepository _planningRepository;
        private readonly AppDbContext _db;

        public ProjectPlanningController(IProjectPlanningRepository planningRepository, AppDbContext db)
        {
            _planningRepository = planningRepository;
            _db = db;
        }

        public async Task<IActionResult> Calendar(Guid? projectId, int? year, int? month)
        {
            var effectiveProjectId = await ResolveProjectIdAsync(projectId, "Calendar");
            if (effectiveProjectId == null) return RedirectToAction("Index", "CharityProjects");

            var today = DateTime.Today;
            var vm = await _planningRepository.BuildCalendarAsync(effectiveProjectId.Value, year ?? today.Year, month ?? today.Month);
            if (vm == null) return NotFound();
            return View(vm);
        }

        public async Task<IActionResult> Timeline(Guid? projectId)
        {
            var effectiveProjectId = await ResolveProjectIdAsync(projectId, "Timeline");
            if (effectiveProjectId == null) return RedirectToAction("Index", "CharityProjects");

            var vm = await _planningRepository.BuildTimelineAsync(effectiveProjectId.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        public async Task<IActionResult> Tracking(Guid? projectId)
        {
            var effectiveProjectId = await ResolveProjectIdAsync(projectId, "Tracking");
            if (effectiveProjectId == null) return RedirectToAction("Index", "CharityProjects");

            var vm = await _planningRepository.BuildTrackingBoardAsync(effectiveProjectId.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        private async Task<Guid?> ResolveProjectIdAsync(Guid? projectId, string screenName)
        {
            if (projectId.HasValue && projectId.Value != Guid.Empty)
                return projectId.Value;

            var firstProjectId = await _db.Set<CharityProject>()
                .AsNoTracking()
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Name)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();

            if (!firstProjectId.HasValue)
            {
                TempData["Error"] = "لا توجد مشروعات متاحة لعرض شاشة التخطيط.";
                return null;
            }

            TempData["Info"] = $"تم فتح أول مشروع متاح لأن شاشة {screenName} تحتاج اختيار مشروع.";
            return firstProjectId.Value;
        }
    }
}
