using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class VolunteerSkillDefinitionsController : Controller
    {
        private readonly IVolunteerSkillDefinitionRepository _repo;
        public VolunteerSkillDefinitionsController(IVolunteerSkillDefinitionRepository repo) => _repo = repo;

        public async Task<IActionResult> Index()
        {
            var items = (await _repo.GetAllAsync()).Select(x => new VolunteerSkillDefinitionListItemVm
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            }).ToList();
            return View(items);
        }

        public IActionResult Create() => View(new CreateVolunteerSkillDefinitionVm { IsActive = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVolunteerSkillDefinitionVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _repo.AddAsync(new VolunteerSkillDefinition
            {
                Id = Guid.NewGuid(),
                Name = vm.Name.Trim(),
                Category = vm.Category?.Trim(),
                SortOrder = vm.SortOrder,
                IsActive = vm.IsActive
            });
            TempData["Success"] = "تم حفظ المهارة.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return View(new EditVolunteerSkillDefinitionVm
            {
                Id = entity.Id,
                Name = entity.Name,
                Category = entity.Category,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVolunteerSkillDefinitionVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var entity = await _repo.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            entity.Name = vm.Name.Trim();
            entity.Category = vm.Category?.Trim();
            entity.SortOrder = vm.SortOrder;
            entity.IsActive = vm.IsActive;
            await _repo.UpdateAsync(entity);
            TempData["Success"] = "تم تحديث المهارة.";
            return RedirectToAction(nameof(Index));
        }
    }
}
