using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.FiscalPeriodClosing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class FiscalPeriodClosingController : Controller
    {
        private readonly AppDbContext _db;

        public FiscalPeriodClosingController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var periods = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

            var result = new FiscalPeriodClosingIndexVm();

            foreach (var p in periods)
            {
                var entries = await _db.Set<JournalEntry>()
                    .AsNoTracking()
                    .Include(x => x.Lines)
                    .Where(x => x.EntryDateUtc >= p.StartDate && x.EntryDateUtc <= p.EndDate)
                    .ToListAsync(ct);

                result.Periods.Add(new FiscalPeriodClosingRowVm
                {
                    Id = p.Id,
                    PeriodName = p.Name,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsClosed = p.IsClosed,
                    ClosedAtUtc = p.ClosedAtUtc,
                    ClosingNotes = p.ClosingNotes,
                    JournalEntriesCount = entries.Count,
                    TotalDebit = entries.SelectMany(x => x.Lines).Sum(x => x.DebitAmount),
                    TotalCredit = entries.SelectMany(x => x.Lines).Sum(x => x.CreditAmount)
                });
            }

            return View(result);
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var vm = await BuildDetailsVmAsync(id, ct);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(CloseFiscalPeriodVm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = model.FiscalPeriodId });

            var period = await _db.Set<FiscalPeriod>()
                .FirstOrDefaultAsync(x => x.Id == model.FiscalPeriodId, ct);

            if (period == null)
                return NotFound();

            if (period.IsClosed)
            {
                TempData["Error"] = "الفترة المالية مقفلة بالفعل.";
                return RedirectToAction(nameof(Details), new { id = model.FiscalPeriodId });
            }

            var details = await BuildDetailsVmAsync(model.FiscalPeriodId, ct);
            if (details == null)
                return NotFound();

            if (details.DraftEntriesCount > 0)
            {
                TempData["Error"] = "لا يمكن إقفال الفترة لوجود قيود غير مرحلة.";
                return RedirectToAction(nameof(Details), new { id = model.FiscalPeriodId });
            }

            if (details.Difference != 0)
            {
                TempData["Error"] = "لا يمكن إقفال الفترة لأن إجمالي المدين لا يساوي إجمالي الدائن.";
                return RedirectToAction(nameof(Details), new { id = model.FiscalPeriodId });
            }

            period.IsClosed = true;
            period.ClosedAtUtc = DateTime.UtcNow;
            period.ClosedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            period.ClosingNotes = model.ClosingNotes;

            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم إقفال الفترة المالية بنجاح.";
            return RedirectToAction(nameof(Details), new { id = model.FiscalPeriodId });
        }

        private async Task<FiscalPeriodClosingDetailsVm?> BuildDetailsVmAsync(Guid fiscalPeriodId, CancellationToken ct)
        {
            var period = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == fiscalPeriodId, ct);

            if (period == null)
                return null;

            var entries = await _db.Set<JournalEntry>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.EntryDateUtc >= period.StartDate && x.EntryDateUtc <= period.EndDate)
                .OrderBy(x => x.EntryDateUtc)
                .ThenBy(x => x.EntryNumber)
                .ToListAsync(ct);

            var vm = new FiscalPeriodClosingDetailsVm
            {
                FiscalPeriodId = period.Id,
                PeriodName = period.Name,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                IsClosed = period.IsClosed,
                ClosedAtUtc = period.ClosedAtUtc,
                ClosedByUserId = period.ClosedByUserId,
                ClosingNotes = period.ClosingNotes,
                JournalEntriesCount = entries.Count,
                PostedEntriesCount = entries.Count(x => x.Status == JournalEntryStatus.Posted),
                DraftEntriesCount = entries.Count(x => x.Status == JournalEntryStatus.Draft),
                TotalDebit = entries.SelectMany(x => x.Lines).Sum(x => x.DebitAmount),
                TotalCredit = entries.SelectMany(x => x.Lines).Sum(x => x.CreditAmount)
            };

            vm.Entries = entries.Select(x => new FiscalPeriodClosingEntryRowVm
            {
                Id = x.Id,
                EntryNumber = x.EntryNumber,
                EntryDateUtc = x.EntryDateUtc,
                Description = x.Description,
                Status = x.Status,
                SourceType = x.SourceType,
                SourceNumber = x.SourceNumber,
                TotalDebit = x.Lines.Sum(l => l.DebitAmount),
                TotalCredit = x.Lines.Sum(l => l.CreditAmount)
            }).ToList();

            return vm;
        }
    }
}
