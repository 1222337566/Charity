using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.Controllers
{
    public class VolunteerSkillsController : Controller
    {
        private readonly IVolunteerSkillRepository _repo;
        private readonly IVolunteerRepository _volunteerRepo;
        private readonly IVolunteerSkillDefinitionRepository _definitionRepo;

        public VolunteerSkillsController(
            IVolunteerSkillRepository repo,
            IVolunteerRepository volunteerRepo,
            IVolunteerSkillDefinitionRepository definitionRepo)
        {
            _repo = repo;
            _volunteerRepo = volunteerRepo;
            _definitionRepo = definitionRepo;
        }

        public async Task<IActionResult> Index(Guid volunteerId)
        {
            var volunteer = await _volunteerRepo.GetByIdAsync(volunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;

            var items = (await _repo.GetByVolunteerIdAsync(volunteerId)).Select(x => new VolunteerSkillListItemVm
            {
                Id = x.Id,
                SkillName = x.SkillDefinition?.Name ?? string.Empty,
                Category = x.SkillDefinition?.Category,
                SkillLevel = x.SkillLevel,
                Notes = x.Notes
            }).ToList();
            return View(items);
        }

        public async Task<IActionResult> Create(Guid volunteerId)
        {
            var volunteer = await _volunteerRepo.GetByIdAsync(volunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            var vm = new CreateVolunteerSkillVm { VolunteerId = volunteerId };
            await FillLookups(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVolunteerSkillVm vm)
        {
            var volunteer = await _volunteerRepo.GetByIdAsync(vm.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View(vm);
            }
            await _repo.AddAsync(new VolunteerSkill
            {
                Id = Guid.NewGuid(),
                VolunteerId = vm.VolunteerId,
                SkillDefinitionId = vm.SkillDefinitionId,
                SkillLevel = vm.SkillLevel,
                Notes = vm.Notes?.Trim()
            });
            TempData["Success"] = "تم حفظ مهارة المتطوع.";
            return RedirectToAction(nameof(Index), new { volunteerId = vm.VolunteerId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            var volunteer = await _volunteerRepo.GetByIdAsync(entity.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            var vm = new EditVolunteerSkillVm
            {
                Id = entity.Id,
                VolunteerId = entity.VolunteerId,
                SkillDefinitionId = entity.SkillDefinitionId,
                SkillLevel = entity.SkillLevel,
                Notes = entity.Notes
            };
            await FillLookups(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVolunteerSkillVm vm)
        {
            var entity = await _repo.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            var volunteer = await _volunteerRepo.GetByIdAsync(entity.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View(vm);
            }
            entity.SkillDefinitionId = vm.SkillDefinitionId;
            entity.SkillLevel = vm.SkillLevel;
            entity.Notes = vm.Notes?.Trim();
            await _repo.UpdateAsync(entity);
            TempData["Success"] = "تم تحديث المهارة.";
            return RedirectToAction(nameof(Index), new { volunteerId = entity.VolunteerId });
        }

        private async Task FillLookups(CreateVolunteerSkillVm vm)
        {
            vm.SkillDefinitions = (await _definitionRepo.GetAllAsync())
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList();
            vm.SkillLevels = new List<SelectListItem>
            {
                new("مبتدئ", "Beginner"),
                new("متوسط", "Intermediate"),
                new("متقدم", "Advanced")
            };
        }
    }
}
