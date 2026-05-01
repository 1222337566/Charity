using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class WorkflowBoardController : Controller
    {
        private readonly IWorkflowService             _workflow;
        private readonly AppDbContext _db;
        private readonly IWorkflowCompletionHandler   _completionHandler;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkflowBoardController(
            IWorkflowService workflow,
            IWorkflowCompletionHandler completionHandler,
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
        {
            _workflow          = workflow;
            _completionHandler = completionHandler;
            _userManager       = userManager;
            _db = db;
        }
        [HttpGet]
        public IActionResult OpenEntityDirect(string entityType, Guid entityId)
        {
            if (entityId == Guid.Empty)
                return RedirectToAction(nameof(Index));

            return RedirectToEntity(entityType, entityId);
        }
        // ── لوحة المهام ──
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var user  = await _userManager.GetUserAsync(User);
            var roles = user != null
                ? (await _userManager.GetRolesAsync(user)).ToList()
                : new List<string>();

            // 1. جيب الخطوات المعلقة على هذا المستخدم
            var pendingSteps = await _workflow.GetPendingForRolesAsync(roles, 200, ct);

            // 2. لكل كيان فريد — جيب كل خطواته (للخط الزمني الكامل)
            var allStepsByEntity = new Dictionary<(string EntityType, Guid EntityId), List<WorkflowStep>>();
            var entityKeys = pendingSteps
                .Select(s => (s.EntityType, s.EntityId))
                .Distinct().ToList();

            foreach (var key in entityKeys)
            {
                var allSteps = await _workflow.GetStepsAsync(key.EntityType, key.EntityId, ct);
                allStepsByEntity[key] = allSteps.OrderBy(s => s.StepOrder).ToList();
            }

            ViewBag.Roles           = roles;
            ViewBag.AllStepsByEntity = allStepsByEntity;
            return View(pendingSteps);
        }
        [HttpGet]
        public async Task<IActionResult> OpenEntity(Guid stepId)
        {
            var step = await _db.WorkflowSteps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == stepId);

            if (step == null)
                return NotFound();

            return RedirectToEntity(step.EntityType, step.EntityId);
        }

        private IActionResult RedirectToEntity(string? entityType, Guid entityId)
        {
            var type = (entityType ?? string.Empty).Trim();

            return type switch
            {
                "HumanitarianResearch" or "BeneficiaryHumanitarianResearch" =>
                    RedirectToAction("Details", "BeneficiaryHumanitarianResearchs", new { id = entityId }),

                "AidRequest" or "BeneficiaryAidRequest" =>
                    RedirectToAction("Details", "BeneficiaryAidRequests", new { id = entityId }),

                "Donation" or "CharityDonation" =>
                    RedirectToAction("Details", "Donations", new { id = entityId }),

                "Disbursement" or "BeneficiaryAidDisbursement" =>
                    RedirectToAction("Details", "BeneficiaryAidDisbursements", new { id = entityId }),

                "StoreIssue" or "CharityStoreIssue" =>
                    RedirectToAction("Details", "CharityStoreIssues", new { id = entityId }),

                "StoreReceipt" or "CharityStoreReceipt" =>
                    RedirectToAction("Details", "CharityStoreReceipts", new { id = entityId }),

                _ => RedirectToAction("Index", "WorkflowBoard")
            };
        }
        // ── تفاصيل مسار طلب ──
        public async Task<IActionResult> Steps(string entityType, Guid entityId, CancellationToken ct)
        {
            var steps = await _workflow.GetStepsAsync(entityType, entityId, ct);
            ViewBag.EntityType = entityType;
            ViewBag.EntityId   = entityId;
            return View(steps);
        }

        // ── اتخاذ إجراء ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeAction(
            Guid stepId, string action, string? note,
            string returnUrl, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User) ?? "";
            var result = await _workflow.TakeActionAsync(stepId, action, userId, note, ct);

            TempData["WorkflowSuccess"] = result.Success;
            TempData["WorkflowMessage"] = result.Message;

            if (result.Success && result.EntityType != null && result.EntityId.HasValue)
            {
                if (result.WorkflowCompleted)
                {
                    await _completionHandler.OnCompletedAsync(
                        result.EntityType, result.EntityId.Value, userId, ct);
                    TempData["WorkflowMessage"] =
                        $"✅ اكتمل مسار الموافقة — تم اعتماد {EntityAr(result.EntityType)} تلقائياً";
                }
                else if (result.WorkflowRejected)
                {
                    await _completionHandler.OnRejectedAsync(
                        result.EntityType, result.EntityId.Value, userId, note, ct);
                    TempData["WorkflowMessage"] =
                        $"❌ تم رفض {EntityAr(result.EntityType)} — حُدِّثت الحالة تلقائياً";
                }
            }

            return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/WorkflowBoard" : returnUrl);
        }

        private static string EntityAr(string t) => t switch {
            "AidRequest"           => "طلب المساعدة",
            "HumanitarianResearch" => "البحث الإنساني",
            "ProjectProposal"      => "مقترح المشروع",
            "AidCycle"             => "دورة الصرف",
            "KafalaCase"           => "الكفالة",
            _ => t
        };
    }
}
