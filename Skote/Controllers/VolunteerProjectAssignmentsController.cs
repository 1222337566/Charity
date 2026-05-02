using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class VolunteerProjectAssignmentsController : Controller
{
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IVolunteerProjectAssignmentRepository _repository;
    private readonly AppDbContext _db;

    public VolunteerProjectAssignmentsController(
        IVolunteerRepository volunteerRepository,
        IVolunteerProjectAssignmentRepository repository,
        AppDbContext db)
    {
        _volunteerRepository = volunteerRepository;
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
            var model = items.Select(MapAssignment).ToList();
            return View(model);
        }

        ViewBag.ShowVolunteerColumn = true;
        var rows = await _db.Set<VolunteerProjectAssignment>()
            .AsNoTracking()
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.RoleTitle)
            .Select(x => new
            {
                x.Id,
                x.VolunteerId,
                x.ProjectNameSnapshot,
                x.RoleTitle,
                x.AssignmentType,
                x.StartDate,
                x.EndDate,
                x.TargetHours,
                x.Status,
                VolunteerName = x.Volunteer != null ? x.Volunteer.FullName : string.Empty
            })
            .ToListAsync();

        ViewBag.VolunteerNamesByAssignmentId = rows.ToDictionary(x => x.Id, x => x.VolunteerName);
        ViewBag.VolunteerIdsByAssignmentId = rows.ToDictionary(x => x.Id, x => x.VolunteerId);
        return View(rows.Select(x => new VolunteerAssignmentListItemVm
        {
            Id = x.Id,
            VolunteerId = x.VolunteerId,
            ProjectName = x.ProjectNameSnapshot ?? "-",
            RoleTitle = x.RoleTitle,
            AssignmentType = x.AssignmentType,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            TargetHours = x.TargetHours,
            Status = x.Status
        }).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid volunteerId)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId);
        if (volunteer == null) return NotFound();

        var vm = new CreateVolunteerProjectAssignmentVm
        {
            VolunteerId = volunteerId
        };
        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVolunteerProjectAssignmentVm vm)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(vm.VolunteerId);
        if (volunteer == null) return NotFound();

        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        if (!ModelState.IsValid) return View(vm);

        var projectName = await _db.Set<CharityProject>()
            .Where(x => x.Id == vm.ProjectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        await _repository.AddAsync(new VolunteerProjectAssignment
        {
            Id = Guid.NewGuid(),
            VolunteerId = vm.VolunteerId,
            ProjectId = vm.ProjectId,
            ProjectNameSnapshot = projectName,
            RoleTitle = vm.RoleTitle.Trim(),
            AssignmentType = vm.AssignmentType,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            TargetHours = vm.TargetHours,
            Status = vm.Status,
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم إضافة إسناد المتطوع";
        return RedirectToAction(nameof(Index), new { volunteerId = vm.VolunteerId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var volunteer = await _volunteerRepository.GetByIdAsync(entity.VolunteerId);
        if (volunteer == null) return NotFound();

        var vm = new EditVolunteerProjectAssignmentVm
        {
            Id = entity.Id,
            VolunteerId = entity.VolunteerId,
            ProjectId = entity.ProjectId,
            RoleTitle = entity.RoleTitle,
            AssignmentType = entity.AssignmentType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TargetHours = entity.TargetHours,
            Status = entity.Status,
            Notes = entity.Notes
        };

        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditVolunteerProjectAssignmentVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        var volunteer = await _volunteerRepository.GetByIdAsync(vm.VolunteerId);
        if (volunteer == null) return NotFound();

        await FillLookups(vm);
        ViewBag.Volunteer = volunteer;
        if (!ModelState.IsValid) return View(vm);

        var projectName = await _db.Set<CharityProject>()
            .Where(x => x.Id == vm.ProjectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        entity.ProjectId = vm.ProjectId;
        entity.ProjectNameSnapshot = projectName;
        entity.RoleTitle = vm.RoleTitle.Trim();
        entity.AssignmentType = vm.AssignmentType;
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        entity.TargetHours = vm.TargetHours;
        entity.Status = vm.Status;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل الإسناد";
        return RedirectToAction(nameof(Index), new { volunteerId = vm.VolunteerId });
    }

    private static VolunteerAssignmentListItemVm MapAssignment(VolunteerProjectAssignment x) => new()
    {
        Id = x.Id,
        VolunteerId = x.VolunteerId,
        ProjectName = x.ProjectNameSnapshot ?? "-",
        RoleTitle = x.RoleTitle,
        AssignmentType = x.AssignmentType,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        TargetHours = x.TargetHours,
        Status = x.Status
    };

    private async Task FillLookups(CreateVolunteerProjectAssignmentVm vm)
    {
        vm.Projects = await _db.Set<CharityProject>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        vm.AssignmentTypes = new()
        {
            new SelectListItem("مستمر", "Recurring"),
            new SelectListItem("فعالية", "Event"),
            new SelectListItem("موسمي", "Seasonal")
        };

        vm.Statuses = new()
        {
            new SelectListItem("نشط", "Active"),
            new SelectListItem("مكتمل", "Completed"),
            new SelectListItem("متوقف", "Paused"),
            new SelectListItem("ملغي", "Cancelled")
        };
    }
}
