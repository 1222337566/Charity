using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class VolunteerAvailabilitySlotsController : Controller
    {
        private readonly IVolunteerAvailabilitySlotRepository _repo;
        private readonly IVolunteerRepository _volunteerRepo;
        private readonly AppDbContext _db;

        public VolunteerAvailabilitySlotsController(
            IVolunteerAvailabilitySlotRepository repo,
            IVolunteerRepository volunteerRepo,
            AppDbContext db)
        {
            _repo = repo;
            _volunteerRepo = volunteerRepo;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? volunteerId)
        {
            var hasVolunteerContext = volunteerId.HasValue && volunteerId.Value != Guid.Empty;
            if (hasVolunteerContext)
            {
                var volunteer = await _volunteerRepo.GetByIdAsync(volunteerId!.Value);
                if (volunteer == null) return NotFound();
                ViewBag.Volunteer = volunteer;
                var items = (await _repo.GetByVolunteerIdAsync(volunteerId.Value)).Select(MapSlot).ToList();
                return View(items);
            }

            var rows = await _db.Set<VolunteerAvailabilitySlot>()
                .AsNoTracking()
                .OrderBy(x => x.DayOfWeekName)
                .ThenBy(x => x.FromTime)
                .Select(x => new
                {
                    x.Id,
                    x.VolunteerId,
                    x.DayOfWeekName,
                    x.FromTime,
                    x.ToTime,
                    x.AvailabilityType,
                    x.Area,
                    x.IsRemoteAllowed,
                    x.Notes,
                    VolunteerName = x.Volunteer != null ? x.Volunteer.FullName : string.Empty
                })
                .ToListAsync();
            ViewBag.ShowVolunteerColumn = true;
            ViewBag.VolunteerNamesBySlotId = rows.ToDictionary(x => x.Id, x => x.VolunteerName);
            ViewBag.VolunteerIdsBySlotId = rows.ToDictionary(x => x.Id, x => x.VolunteerId);
            return View(rows.Select(x => new VolunteerAvailabilitySlotListItemVm
            {
                Id = x.Id,
                DayOfWeekName = x.DayOfWeekName,
                TimeText = $"{x.FromTime:hh:mm} - {x.ToTime:hh:mm}",
                AvailabilityType = x.AvailabilityType,
                Area = x.Area,
                IsRemoteAllowed = x.IsRemoteAllowed,
                Notes = x.Notes
            }).ToList());
        }

        public async Task<IActionResult> Create(Guid volunteerId)
        {
            var volunteer = await _volunteerRepo.GetByIdAsync(volunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            var vm = new CreateVolunteerAvailabilitySlotVm { VolunteerId = volunteerId };
            FillLookups(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVolunteerAvailabilitySlotVm vm)
        {
            var volunteer = await _volunteerRepo.GetByIdAsync(vm.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            if (!ModelState.IsValid)
            {
                FillLookups(vm);
                return View(vm);
            }
            await _repo.AddAsync(new VolunteerAvailabilitySlot
            {
                Id = Guid.NewGuid(),
                VolunteerId = vm.VolunteerId,
                DayOfWeekName = vm.DayOfWeekName,
                FromTime = vm.FromTime,
                ToTime = vm.ToTime,
                AvailabilityType = vm.AvailabilityType,
                Area = vm.Area?.Trim(),
                IsRemoteAllowed = vm.IsRemoteAllowed,
                Notes = vm.Notes?.Trim()
            });
            TempData["Success"] = "تم حفظ وقت التوفر.";
            return RedirectToAction(nameof(Index), new { volunteerId = vm.VolunteerId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            var volunteer = await _volunteerRepo.GetByIdAsync(entity.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            var vm = new EditVolunteerAvailabilitySlotVm
            {
                Id = entity.Id,
                VolunteerId = entity.VolunteerId,
                DayOfWeekName = entity.DayOfWeekName,
                FromTime = entity.FromTime,
                ToTime = entity.ToTime,
                AvailabilityType = entity.AvailabilityType,
                Area = entity.Area,
                IsRemoteAllowed = entity.IsRemoteAllowed,
                Notes = entity.Notes
            };
            FillLookups(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVolunteerAvailabilitySlotVm vm)
        {
            var entity = await _repo.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            var volunteer = await _volunteerRepo.GetByIdAsync(entity.VolunteerId);
            if (volunteer == null) return NotFound();
            ViewBag.Volunteer = volunteer;
            if (!ModelState.IsValid)
            {
                FillLookups(vm);
                return View(vm);
            }
            entity.DayOfWeekName = vm.DayOfWeekName;
            entity.FromTime = vm.FromTime;
            entity.ToTime = vm.ToTime;
            entity.AvailabilityType = vm.AvailabilityType;
            entity.Area = vm.Area?.Trim();
            entity.IsRemoteAllowed = vm.IsRemoteAllowed;
            entity.Notes = vm.Notes?.Trim();
            await _repo.UpdateAsync(entity);
            TempData["Success"] = "تم تحديث وقت التوفر.";
            return RedirectToAction(nameof(Index), new { volunteerId = entity.VolunteerId });
        }

        private static VolunteerAvailabilitySlotListItemVm MapSlot(VolunteerAvailabilitySlot x) => new()
        {
            Id = x.Id,
            DayOfWeekName = x.DayOfWeekName,
            TimeText = $"{x.FromTime:hh\\:mm} - {x.ToTime:hh\\:mm}",
            AvailabilityType = x.AvailabilityType,
            Area = x.Area,
            IsRemoteAllowed = x.IsRemoteAllowed,
            Notes = x.Notes
        };

        private void FillLookups(CreateVolunteerAvailabilitySlotVm vm)
        {
            vm.Days = new List<SelectListItem>
            {
                new("السبت", "Saturday"), new("الأحد", "Sunday"), new("الاثنين", "Monday"),
                new("الثلاثاء", "Tuesday"), new("الأربعاء", "Wednesday"), new("الخميس", "Thursday"), new("الجمعة", "Friday")
            };
            vm.Types = new List<SelectListItem>
            {
                new("منتظم", "Regular"), new("عند الحاجة", "Occasional"), new("طوارئ", "Emergency")
            };
        }
    }
}
