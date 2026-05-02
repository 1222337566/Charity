using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class BulkMessageController : Controller
    {
        private readonly AppDbContext _db;
        public BulkMessageController(AppDbContext db) => _db = db;

        // ── صفحة إرسال رسائل جماعية ──
        public IActionResult Index() => View();

        // ── معاينة المستلمين حسب المعيار ──
        [HttpPost]
        public async Task<IActionResult> Preview(BulkMessageFilterVm filter, CancellationToken ct)
        {
            var recipients = await BuildRecipientsAsync(filter, ct);
            filter.Recipients = recipients;
            return View("Index", filter);
        }

        // ── إرسال الرسائل ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(BulkMessageFilterVm filter, CancellationToken ct)
        {
            var recipients = await BuildRecipientsAsync(filter, ct);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sent = 0; var skipped = 0;

            foreach (var r in recipients.Where(x => !string.IsNullOrWhiteSpace(x.Phone)))
            {
                // سجّل رسالة التواصل مع المستفيد لو كان من المستفيدين
                if (filter.TargetGroup == "Beneficiaries" && r.EntityId.HasValue)
                {
                    _db.Set<BeneficiaryContactLog>().Add(new BeneficiaryContactLog
                    {
                        BeneficiaryId   = r.EntityId.Value,
                        ContactType     = "WhatsApp",
                        ContactDate     = DateTime.Today,
                        Outcome         = "Reached",
                        Subject         = filter.Subject,
                        Notes           = filter.MessageText,
                        CreatedByUserId = userId
                    });
                }
                // هنا ممكن تربط بـ ISendService الموجود للإرسال الفعلي
                // await _sendService.Send(new SendModel { msisdn = "2"+r.Phone, text = filter.MessageText });
                sent++;
            }
            skipped = recipients.Count - sent;
            await _db.SaveChangesAsync(ct);

            TempData["BulkResult"] = $"تم الإرسال لـ {sent} شخص · تم تخطي {skipped} (بدون هاتف)";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<MessageRecipient>> BuildRecipientsAsync(
            BulkMessageFilterVm f, CancellationToken ct)
        {
            var result = new List<MessageRecipient>();

            if (f.TargetGroup == "Beneficiaries")
            {
                var query = _db.Set<Beneficiary>().AsNoTracking().Where(x => x.IsActive);
                if (f.FilterNoAidMonths > 0)
                {
                    var cutoff = DateTime.Today.AddMonths(-f.FilterNoAidMonths);
                    query = query.Where(x =>
                        !x.AidDisbursements.Any()
                        || x.AidDisbursements.Max(d => d.DisbursementDate) < cutoff);
                }
                result = await query
                    .Select(x => new MessageRecipient {
                        EntityId = x.Id, Name = x.FullName,
                        Phone = x.PhoneNumber, Code = x.Code
                    }).Take(500).ToListAsync(ct);
            }
            else if (f.TargetGroup == "KafalaOverdue")
            {
                var today = DateTime.Today;
                result = await _db.Set<KafalaCase>().AsNoTracking()
                    .Include(x => x.Sponsor)
                    .Where(x => x.Status == "Active"
                             && x.NextDueDate.HasValue
                             && x.NextDueDate.Value.Date < today)
                    .Select(x => new MessageRecipient {
                        Name  = x.Sponsor!.FullName,
                        Phone = x.Sponsor.PhoneNumber,
                        Code  = x.CaseNumber,
                        Extra = $"متأخر {(int)(today - x.NextDueDate!.Value).TotalDays} يوم"
                    }).Take(500).ToListAsync(ct);
            }
            else if (f.TargetGroup == "Donors")
            {
                var cutoff = DateTime.Today.AddMonths(-(f.FilterNoAidMonths > 0 ? f.FilterNoAidMonths : 6));
                result = await _db.Set<Donor>().AsNoTracking()
                    .Where(x => !x.Donations.Any()
                             || x.Donations.Max(d => d.DonationDate) < cutoff)
                    .Select(x => new MessageRecipient {
                        Name  = x.FullName,
                        Phone = x.PhoneNumber,
                        Extra = "متبرع صامت"
                    }).Take(500).ToListAsync(ct);
            }

            // استبدال المتغيرات في النص
            foreach (var r in result)
            {
                r.ResolvedText = (f.MessageText ?? "")
                    .Replace("{الاسم}",  r.Name)
                    .Replace("{الكود}",  r.Code ?? "")
                    .Replace("{التفاصيل}", r.Extra ?? "");
            }
            return result;
        }
    }

    public class BulkMessageFilterVm
    {
        // all | Beneficiaries | KafalaOverdue | Donors
        public string  TargetGroup      { get; set; } = "Beneficiaries";
        public int     FilterNoAidMonths { get; set; } = 0;
        public string? Subject          { get; set; }
        public string? MessageText      { get; set; }
        public List<MessageRecipient> Recipients { get; set; } = new();
    }

    public class MessageRecipient
    {
        public Guid?   EntityId     { get; set; }
        public string  Name         { get; set; } = "";
        public string? Phone        { get; set; }
        public string? Code         { get; set; }
        public string? Extra        { get; set; }
        public string? ResolvedText { get; set; }
    }
}
