using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectTaskTrackingController : Controller
    {
        private readonly IProjectTaskTrackingRepository _repository;
        private readonly AppDbContext _db;
        public ProjectTaskTrackingController(IProjectTaskTrackingRepository repository, AppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IActionResult> Board(Guid? projectId)
        {
            var effectiveProjectId = await ResolveProjectIdAsync(projectId, "لوحة المهام");
            if (effectiveProjectId == null) return RedirectToAction("Index", "CharityProjects");

            var vm = await _repository.BuildBoardAsync(effectiveProjectId.Value);
            if (vm == null) return NotFound();
            ViewBag.ProjectHeader = vm.ProjectHeader;
            return View(vm);
        }

        public async Task<IActionResult> DailyFollowUp(Guid? projectId, DateTime? date)
        {
            var effectiveProjectId = await ResolveProjectIdAsync(projectId, "المتابعة اليومية");
            if (effectiveProjectId == null) return RedirectToAction("Index", "CharityProjects");

            var vm = await _repository.BuildDailyFollowUpAsync(effectiveProjectId.Value, (date ?? DateTime.Today).Date);
            if (vm == null) return NotFound();
            ViewBag.ProjectHeader = vm.ProjectHeader;
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
                TempData["Error"] = "لا توجد مشروعات متاحة لعرض شاشة المهام.";
                return null;
            }

            TempData["Info"] = $"تم فتح أول مشروع متاح لأن شاشة {screenName} تحتاج اختيار مشروع.";
            return firstProjectId.Value;
        }
    }
}
