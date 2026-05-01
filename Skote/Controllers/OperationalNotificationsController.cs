using InfrastructureManagmentServices.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Skote.Controllers;

[Authorize]
public class OperationalNotificationsController : Controller
{
    private readonly ICharityOperationNotificationService _notificationService;

    public OperationalNotificationsController(ICharityOperationNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendAidRequestSample(CancellationToken ct)
    {
        await _notificationService.NotifyAidRequestCreatedAsync(
            beneficiaryName: "حالة تجريبية",
            aidTypeName: "إعانة نقدية",
            requestedAmount: 1500,
            actorUserId: User.FindFirstValue(ClaimTypes.NameIdentifier),
            url: "/Beneficiaries",
            ct: ct);

        TempData["SuccessMessage"] = "تم إرسال إشعار تجريبي لطلب مساعدة.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendCommitteeDecisionSample(CancellationToken ct)
    {
        await _notificationService.NotifyCommitteeDecisionCreatedAsync(
            beneficiaryName: "حالة تجريبية",
            decisionType: "اعتماد",
            actorUserId: User.FindFirstValue(ClaimTypes.NameIdentifier),
            url: "/Beneficiaries",
            ct: ct);

        TempData["SuccessMessage"] = "تم إرسال إشعار تجريبي لقرار لجنة.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendGrantDueSample(CancellationToken ct)
    {
        await _notificationService.NotifyGrantInstallmentDueSoonAsync(
            agreementTitle: "منحة دعم الحالات الأولى بالرعاية",
            amount: 250000,
            dueDate: DateTime.Today.AddDays(5),
            actorUserId: User.FindFirstValue(ClaimTypes.NameIdentifier),
            url: "/GrantInstallments",
            ct: ct);

        TempData["SuccessMessage"] = "تم إرسال إشعار تجريبي لدفعة تمويل قريبة.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendLowStockSample(CancellationToken ct)
    {
        await _notificationService.NotifyLowStockAsync(
            itemName: "كرتونة مواد غذائية",
            storeName: "المخزن الرئيسي",
            currentBalance: 8,
            minimumBalance: 20,
            actorUserId: User.FindFirstValue(ClaimTypes.NameIdentifier),
            url: "/CharityStoreIssues",
            ct: ct);

        TempData["SuccessMessage"] = "تم إرسال إشعار تجريبي لرصيد منخفض بالمخزن.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendPayrollApprovedSample(CancellationToken ct)
    {
        await _notificationService.NotifyPayrollApprovedAsync(
            year: DateTime.Today.Year,
            month: DateTime.Today.Month,
            totalNetAmount: 125000,
            actorUserId: User.FindFirstValue(ClaimTypes.NameIdentifier),
            url: "/PayrollMonths",
            ct: ct);

        TempData["SuccessMessage"] = "تم إرسال إشعار تجريبي لاعتماد المرتبات.";
        return RedirectToAction(nameof(Index));
    }
}
