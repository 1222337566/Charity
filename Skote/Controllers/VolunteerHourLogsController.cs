using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class VolunteerHourLogsController : Controller
{
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IVolunteerProjectAssignmentRepository _assignmentRepository;
    private readonly IVolunteerHourLogRepository _repository;
    private readonly AppDbContext _db;

    public VolunteerHourLogsController(
        IVolunteerRepository volunteerRepository,
        IVolunteerProjectAssignmentRepository assignmentRepository,
        IVolunteerHourLogRepository repository,
        AppDbContext db)
    {
        _volunteerRepository = volunteerRepository;
        _assignmentRepository = assignmentRepository;
        _repository = repository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? volunteerId)
    {
        var hasVolunteerContext = volunteerId.HasValue && volunteerId.Value != Guid.Empty;
        if (hasVolunteerContext)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId!.Value);
            if (volunteer == null) return NotFound();

            ViewBag.Volunteer = volunteer;

            var items = await _repository.GetByVolunteerIdAsync(volunteerId.Value);
            var model = items.Select(MapHourLog).ToList();
            ViewBag.TotalHours = model.Sum(x => x.Hours);
            return View(model);
        }

        var rows = await _db.Set<VolunteerHourLog>()
            .AsNoTracking()
            .OrderByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new
            {
                x.Id,
                x.VolunteerId,
                x.WorkDate,
                x.Hours,
                x.ActivityTitle,
                x.ProjectNameSnapshot,
                x.Outcome,
                VolunteerName = x.Volunteer != null ? x.Volunteer.FullName : string.Empty
            })
            .ToListAsync();

        ViewBag.ShowVolunteerColumn = true;
        ViewBag.VolunteerNamesByLogId = rows.ToDictionary(x => x.Id, x => x.VolunteerName);
        ViewBag.VolunteerIdsByLogId = rows.ToDictionary(x => x.Id, x => x.VolunteerId);
        ViewBag.TotalHours = rows.Sum(x => x.Hours);
        return View(rows.Select(x => new VolunteerHourLogListItemVm
        {
            Id = x.Id,
            VolunteerId = x.VolunteerId,
            WorkDate = x.WorkDate,
            Hours = x.Hours,
            ActivityTitle = x.ActivityTitle,
            ProjectName = x.ProjectNameSnapshot,
            Outcome = x.Outcome
        }).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid volunteerId)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId);
        if (volunteer == null) return NotFound();

        var vm = new CreateVolunteerHourLogVm
        {
            VolunteerId = volunteerId
        };
        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVolunteerHourLogVm vm)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(vm.VolunteerId);
        if (volunteer == null) return NotFound();

        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        if (!ModelState.IsValid) return View(vm);

        var assignment = vm.AssignmentId.HasValue ? await _assignmentRepository.GetByIdAsync(vm.AssignmentId.Value) : null;

        var projectId = vm.ProjectId ?? assignment?.ProjectId;
        var projectName = await _db.Set<CharityProject>()
            .Where(x => x.Id == projectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        await _repository.AddAsync(new VolunteerHourLog
        {
            Id = Guid.NewGuid(),
            VolunteerId = vm.VolunteerId,
            AssignmentId = vm.AssignmentId,
            ProjectId = projectId,
            ProjectNameSnapshot = projectName,
            WorkDate = vm.WorkDate,
            Hours = vm.Hours,
            ActivityTitle = vm.ActivityTitle.Trim(),
            Outcome = vm.Outcome?.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم تسجيل ساعات التطوع";
        return RedirectToAction(nameof(Index), new { volunteerId = vm.VolunteerId });
    }

    private static VolunteerHourLogListItemVm MapHourLog(VolunteerHourLog x) => new()
    {
        Id = x.Id,
        VolunteerId = x.VolunteerId,
        WorkDate = x.WorkDate,
        Hours = x.Hours,
        ActivityTitle = x.ActivityTitle,
        ProjectName = x.ProjectNameSnapshot,
        Outcome = x.Outcome
    };

    private async Task FillLookups(CreateVolunteerHourLogVm vm)
    {
        vm.Assignments = await _db.Set<VolunteerProjectAssignment>()
            .AsNoTracking()
            .Where(x => x.VolunteerId == vm.VolunteerId)
            .OrderByDescending(x => x.StartDate)
            .Select(x => new SelectListItem(
                $"{x.RoleTitle} - {(x.ProjectNameSnapshot ?? "بدون مشروع")}",
                x.Id.ToString()))
            .ToListAsync();

        vm.Projects = await _db.Set<CharityProject>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();
    }
}
