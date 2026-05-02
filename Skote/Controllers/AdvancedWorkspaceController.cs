using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using InfrastrfuctureManagmentCore.Domains.Charity.Workspace;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Skote.Controllers
{
    [Authorize]
    public class AdvancedWorkspaceController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        private static readonly JsonSerializerOptions _json =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public AdvancedWorkspaceController(AppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ─────────────────────────────────────────────────────────────
        // MAIN PAGE
        // ─────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User) ?? "";
            var layout = await _db.Set<UserWorkspaceLayout>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            ViewBag.LayoutJson = layout?.LayoutJson ?? "[]";

            // Workflow definitions (for designer tab)
            var defs = await _db.Set<DynamicWorkflowDefinition>()
                .AsNoTracking()
                .Include(x => x.Steps.OrderBy(s => s.StepOrder))
                .OrderBy(x => x.EntityType)
                .ToListAsync();
            ViewBag.WorkflowDefs = defs;

            // Available roles for workflow assignment
            ViewBag.AvailableRoles = AvailableRoles();

            // Available entity types for workflow
            ViewBag.EntityTypes = EntityTypeOptions();

            return View();
        }

        // ─────────────────────────────────────────────────────────────
        // WIDGET LAYOUT — SAVE & LOAD
        // ─────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLayout([FromBody] SaveLayoutRequest req)
        {
            if (req?.LayoutJson == null)
                return Json(new { success = false });

            var userId = _userManager.GetUserId(User) ?? "";
            var existing = await _db.Set<UserWorkspaceLayout>()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
            {
                _db.Set<UserWorkspaceLayout>().Add(new UserWorkspaceLayout
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LayoutJson = req.LayoutJson
                });
            }
            else
            {
                existing.LayoutJson = req.LayoutJson;
                existing.UpdatedAtUtc = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ─────────────────────────────────────────────────────────────
        // WIDGET DATA — AJAX ENDPOINTS
        // ─────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetWidgetData(string key)
        {
            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);
            var soon = today.AddDays(14);

            object data = key switch
            {
                "PendingAidRequests" => new
                {
                    count = await _db.Set<BeneficiaryAidRequest>()
                        .CountAsync(x => x.Status == "Pending" || x.Status == "Submitted")
                },
                "NewBeneficiaries" => new
                {
                    count = await _db.Set<Beneficiary>()
                        .CountAsync(x => x.CreatedAtUtc >= weekAgo)
                },
                "WaitingListTotal" => new
                {
                    count = await _db.Set<BeneficiaryCategoryAssignment>()
                        .CountAsync(x => x.Status == "Waiting")
                },
                "PendingWorkflowSteps" => new
                {
                    count = await _db.Set<WorkflowStep>()
                        .CountAsync(x => x.Status == "Pending" && x.IsActive)
                },
                "DonationSummary" => new
                {
                    count = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation>()
                        .CountAsync(x => x.CreatedAtUtc >= weekAgo)
                },
                "LowStock" => new
                {
                    count = await _db.Set<ItemWarehouseBalance>()
                        .CountAsync(x => x.AvailableQuantity <= x.QuantityOnHand)
                },
                "GrantsDueSoon" => new
                {
                    count = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Funders.GrantInstallment>()
                        .CountAsync(x => !x.IsPaid && x.DueDate <= soon && x.DueDate >= today)
                },
                "ProjectsEndingSoon" => new
                {
                    count = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Projects.CharityProject>()
                        .CountAsync(x => x.EndDate.HasValue && x.EndDate.Value <= soon && x.EndDate.Value >= today)
                },
                "PendingStoreIssues" => new
                {
                    count = await _db.Set<CharityStoreIssue>()
                        .CountAsync(x => x.ApprovalStatus == "Pending" || x.ApprovalStatus == "Draft")
                },
                "HrAttendanceMissing" => new
                {
                    count = await _db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrAttendanceRecord>()
                        .CountAsync(x => x.AttendanceDate == DateTime.Today.Date
                                      && (x.CheckInTime == null || x.CheckOutTime == null))
                },
                _ => new { count = 0 }
            };

            return Json(data);
        }

        // ─────────────────────────────────────────────────────────────
        // LOOKUP DATA MANAGEMENT
        // ─────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetLookups(string type)
        {
            object items = type switch
            {
                "AidType" => await _db.Set<AidTypeLookup>()
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync(),
                "BeneficiaryStatus" => await _db.Set<BeneficiaryStatusLookup>()
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync(),
                "Governorate" => await _db.Set<Governorate>()
                    .OrderBy(x => x.NameAr).ToListAsync(),
                "City" => await _db.Set<City>()
                    .Include(x => x.Governorate)
                    .OrderBy(x => x.NameAr).ToListAsync(),
                "Gender" => await _db.Set<GenderLookup>()
                    .OrderBy(x => x.NameAr).ToListAsync(),
                "MaritalStatus" => await _db.Set<MaritalStatusLookup>()
                    .OrderBy(x => x.NameAr).ToListAsync(),
                _ => Array.Empty<object>()
            };

            return Json(items, _json);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLookup([FromBody] LookupSaveRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.NameAr) || string.IsNullOrWhiteSpace(req.Type))
                return Json(new { success = false, message = "الاسم ونوع القائمة مطلوبان" });

            switch (req.Type)
            {
                case "AidType":
                    if (req.Id.HasValue)
                    {
                        var e = await _db.Set<AidTypeLookup>().FindAsync(req.Id.Value);
                        if (e != null) { e.NameAr = req.NameAr; e.NameEn = req.NameEn; e.IsActive = req.IsActive; }
                    }
                    else
                        _db.Set<AidTypeLookup>().Add(new AidTypeLookup
                        { Id = Guid.NewGuid(), NameAr = req.NameAr, NameEn = req.NameEn, IsActive = req.IsActive });
                    break;

                case "BeneficiaryStatus":
                    if (req.Id.HasValue)
                    {
                        var e = await _db.Set<BeneficiaryStatusLookup>().FindAsync(req.Id.Value);
                        if (e != null) { e.NameAr = req.NameAr; e.IsActive = req.IsActive; }
                    }
                    else
                        _db.Set<BeneficiaryStatusLookup>().Add(new BeneficiaryStatusLookup
                        { Id = Guid.NewGuid(), NameAr = req.NameAr, IsActive = req.IsActive });
                    break;

                case "Governorate":
                    if (req.Id.HasValue)
                    {
                        var e = await _db.Set<Governorate>().FindAsync(req.Id.Value);
                        if (e != null) { e.NameAr = req.NameAr; e.IsActive = req.IsActive; }
                    }
                    else
                        _db.Set<Governorate>().Add(new Governorate
                        { Id = Guid.NewGuid(), NameAr = req.NameAr, IsActive = req.IsActive });
                    break;
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLookup([FromBody] LookupDeleteRequest req)
        {
            try
            {
                switch (req.Type)
                {
                    case "AidType":
                        var a = await _db.Set<AidTypeLookup>().FindAsync(req.Id);
                        if (a != null) _db.Remove(a);
                        break;
                    case "BeneficiaryStatus":
                        var b = await _db.Set<BeneficiaryStatusLookup>().FindAsync(req.Id);
                        if (b != null) _db.Remove(b);
                        break;
                    case "Governorate":
                        var g = await _db.Set<Governorate>().FindAsync(req.Id);
                        if (g != null) _db.Remove(g);
                        break;
                }
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "لا يمكن حذف هذا العنصر لارتباطه ببيانات أخرى" });
            }
        }

        // ─────────────────────────────────────────────────────────────
        // WORKFLOW DESIGNER
        // ─────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWorkflowDefinition([FromBody] WorkflowDefRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.EntityType) || string.IsNullOrWhiteSpace(req.DisplayName))
                return Json(new { success = false, message = "نوع الكيان والاسم مطلوبان" });

            var userId = _userManager.GetUserId(User) ?? "";
            DynamicWorkflowDefinition def;

            if (req.Id.HasValue)
            {
                def = await _db.Set<DynamicWorkflowDefinition>().FindAsync(req.Id.Value)
                      ?? new DynamicWorkflowDefinition();
                def.DisplayName = req.DisplayName;
                def.Notes       = req.Notes;
                def.IsActive    = req.IsActive;
            }
            else
            {
                def = new DynamicWorkflowDefinition
                {
                    Id = Guid.NewGuid(),
                    EntityType = req.EntityType,
                    DisplayName = req.DisplayName,
                    Notes = req.Notes,
                    IsActive = req.IsActive,
                    CreatedByUserId = userId
                };
                _db.Set<DynamicWorkflowDefinition>().Add(def);
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, id = def.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWorkflowStep([FromBody] WorkflowStepRequest req)
        {
            if (req.DefinitionId == Guid.Empty)
                return Json(new { success = false });

            DynamicWorkflowStepTemplate step;

            if (req.Id.HasValue)
            {
                step = await _db.Set<DynamicWorkflowStepTemplate>().FindAsync(req.Id.Value)
                       ?? new DynamicWorkflowStepTemplate();
                step.StepName     = req.StepName;
                step.AssignedRole = req.AssignedRole;
                step.StepOrder    = req.StepOrder;
                step.Description  = req.Description;
            }
            else
            {
                // احسب أعلى ترتيب حالي
                var maxOrder = await _db.Set<DynamicWorkflowStepTemplate>()
                    .Where(x => x.DefinitionId == req.DefinitionId)
                    .Select(x => (int?)x.StepOrder)
                    .MaxAsync() ?? 0;

                step = new DynamicWorkflowStepTemplate
                {
                    Id           = Guid.NewGuid(),
                    DefinitionId = req.DefinitionId,
                    StepName     = req.StepName,
                    AssignedRole = req.AssignedRole,
                    StepOrder    = req.StepOrder > 0 ? req.StepOrder : maxOrder + 1,
                    Description  = req.Description
                };
                _db.Set<DynamicWorkflowStepTemplate>().Add(step);
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, step = new
            {
                step.Id, step.StepName, step.AssignedRole,
                step.StepOrder, step.Description
            }});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkflowStep([FromBody] IdRequest req)
        {
            var step = await _db.Set<DynamicWorkflowStepTemplate>().FindAsync(req.Id);
            if (step != null) _db.Remove(step);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkflowDefinition([FromBody] IdRequest req)
        {
            var def = await _db.Set<DynamicWorkflowDefinition>()
                .Include(x => x.Steps)
                .FirstOrDefaultAsync(x => x.Id == req.Id);
            if (def != null) _db.Remove(def);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderWorkflowSteps([FromBody] ReorderRequest req)
        {
            foreach (var item in req.Items)
            {
                var step = await _db.Set<DynamicWorkflowStepTemplate>().FindAsync(item.Id);
                if (step != null) step.StepOrder = item.Order;
            }
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ─────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────

        private static List<string> AvailableRoles() => new()
        {
            "Admin", "CharityManager", "FinancialOfficer",
            "BeneficiariesOfficer", "SocialResearcher", "StoreKeeper",
            "HrOfficer", "ProjectManager", "AuditOfficer"
        };

        private static List<(string Key, string Label)> EntityTypeOptions() => new()
        {
            ("AidRequest",           "طلب المساعدة"),
            ("HumanitarianResearch", "البحث الإنساني"),
            ("KafalaCase",           "الكفالة"),
            ("AidCycle",             "دورة الصرف"),
            ("ProjectProposal",      "مقترح مشروع"),
            ("StockNeedRequest",     "طلب احتياج"),
            ("StoreIssue",           "إذن صرف مخزني"),
            ("Donation",             "تبرع"),
        };

        // ─────────────────────────────────────────────────────────────
        // REQUEST MODELS
        // ─────────────────────────────────────────────────────────────

        public class SaveLayoutRequest      { public string? LayoutJson { get; set; } }
        public class IdRequest              { public Guid Id { get; set; } }
        public class LookupDeleteRequest    { public Guid Id { get; set; } public string Type { get; set; } = ""; }

        public class LookupSaveRequest
        {
            public Guid?   Id       { get; set; }
            public string  Type     { get; set; } = "";
            public string  NameAr   { get; set; } = "";
            public string? NameEn   { get; set; }
            public bool    IsActive { get; set; } = true;
        }

        public class WorkflowDefRequest
        {
            public Guid?   Id          { get; set; }
            public string  EntityType  { get; set; } = "";
            public string  DisplayName { get; set; } = "";
            public string? Notes       { get; set; }
            public bool    IsActive    { get; set; } = true;
        }

        public class WorkflowStepRequest
        {
            public Guid?   Id           { get; set; }
            public Guid    DefinitionId { get; set; }
            public string  StepName     { get; set; } = "";
            public string  AssignedRole { get; set; } = "";
            public int     StepOrder    { get; set; }
            public string? Description  { get; set; }
        }

        public class ReorderRequest
        {
            public List<ReorderItem> Items { get; set; } = new();
        }
        public class ReorderItem { public Guid Id { get; set; } public int Order { get; set; } }
    }
}
