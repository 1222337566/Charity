using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.Controllers
{
    public class VolunteerDirectoryController : Controller
    {
        private readonly IVolunteerDirectoryRepository _directoryRepo;
        private readonly IVolunteerSkillRepository _skillRepo;
        private readonly IVolunteerSkillDefinitionRepository _definitionRepo;
        private readonly IVolunteerAvailabilitySlotRepository _availabilityRepo;

        public VolunteerDirectoryController(
            IVolunteerDirectoryRepository directoryRepo,
            IVolunteerSkillRepository skillRepo,
            IVolunteerSkillDefinitionRepository definitionRepo,
            IVolunteerAvailabilitySlotRepository availabilityRepo)
        {
            _directoryRepo = directoryRepo;
            _skillRepo = skillRepo;
            _definitionRepo = definitionRepo;
            _availabilityRepo = availabilityRepo;
        }

        public async Task<IActionResult> Index(string? q, Guid? skillDefinitionId, string? area, bool activeOnly = true)
        {
            var vm = new VolunteerDirectorySearchVm
            {
                Q = q,
                SkillDefinitionId = skillDefinitionId,
                Area = area,
                ActiveOnly = activeOnly,
                SkillDefinitions = (await _definitionRepo.GetAllAsync()).Where(x => x.IsActive)
                    .Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList()
            };

            vm.Items = (await _directoryRepo.SearchAsync(q, skillDefinitionId, area, activeOnly)).Select(x => new VolunteerDirectoryItemVm
            {
                Id = x.Id,
                VolunteerCode = x.VolunteerCode,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                PreferredArea = x.PreferredArea,
                IsActive = x.IsActive,
                AssignmentsCount = x.Assignments?.Count ?? 0,
                TotalHours = x.HourLogs?.Sum(h => h.Hours) ?? 0
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Profile(Guid id)
        {
            var volunteer = await _directoryRepo.GetProfileAsync(id);
            if (volunteer == null) return NotFound();

            var vm = new VolunteerProfileVm
            {
                Id = volunteer.Id,
                VolunteerCode = volunteer.VolunteerCode,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                PreferredArea = volunteer.PreferredArea,
                IsActive = volunteer.IsActive,
                AssignmentsCount = volunteer.Assignments?.Count ?? 0,
                TotalHours = volunteer.HourLogs?.Sum(h => h.Hours) ?? 0,
                Skills = (await _skillRepo.GetByVolunteerIdAsync(id)).Select(x => new VolunteerSkillListItemVm
                {
                    Id = x.Id,
                    SkillName = x.SkillDefinition?.Name ?? string.Empty,
                    Category = x.SkillDefinition?.Category,
                    SkillLevel = x.SkillLevel,
                    Notes = x.Notes
                }).ToList(),
                Availability = (await _availabilityRepo.GetByVolunteerIdAsync(id)).Select(x => new VolunteerAvailabilitySlotListItemVm
                {
                    Id = x.Id,
                    DayOfWeekName = x.DayOfWeekName,
                    TimeText = $"{x.FromTime:hh:mm} - {x.ToTime:hh:mm}",
                    AvailabilityType = x.AvailabilityType,
                    Area = x.Area,
                    IsRemoteAllowed = x.IsRemoteAllowed,
                    Notes = x.Notes
                }).ToList()
            };

            return View(vm);
        }
    }
}
