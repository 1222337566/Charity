using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Kafala
{
    public class KafalaAidCycleBridgeService : IKafalaAidCycleBridgeService
    {
        private readonly AppDbContext _db;
        private readonly IKafalaCaseRepository _kafalaCaseRepository;

        public KafalaAidCycleBridgeService(AppDbContext db, IKafalaCaseRepository kafalaCaseRepository)
        {
            _db = db;
            _kafalaCaseRepository = kafalaCaseRepository;
        }

        public async Task<(Guid? aidCycleId, int generatedCount, List<string> messages)> CreateDueSponsorshipCycleAsync(DateTime plannedDisbursementDateUtc, Guid? sponsorId = null)
        {
            var messages = new List<string>();
            var effectiveDate = plannedDisbursementDateUtc.Date;
            var dueCases = await _kafalaCaseRepository.GetActiveDueCasesAsync(effectiveDate);
            if (sponsorId.HasValue)
                dueCases = dueCases.Where(x => x.SponsorId == sponsorId.Value).ToList();

            if (dueCases.Count == 0)
                return (null, 0, new List<string> { "لا توجد كفالات مستحقة في التاريخ المحدد" });

            var sponsorName = dueCases.Select(x => x.Sponsor?.FullName).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            var sponsorshipAidTypeId = await EnsureSponsorshipAidTypeAsync();
            var existingCycle = await FindExistingCycleAsync(effectiveDate, sponsorId);
            var cycle = existingCycle ?? new AidCycle
            {
                Id = Guid.NewGuid(),
                CycleNumber = $"KAF-{effectiveDate:yyyyMM}-{(sponsorId.HasValue ? dueCases[0].SponsorId.ToString()[..4].ToUpperInvariant() : "ALL")}",
                Title = sponsorId.HasValue && !string.IsNullOrWhiteSpace(sponsorName)
                    ? $"دورة كفالات {effectiveDate:yyyy/MM} - {sponsorName}"
                    : $"دورة كفالات {effectiveDate:yyyy/MM}",
                CycleType = "Sponsorship",
                AidTypeId = sponsorshipAidTypeId,
                PeriodYear = effectiveDate.Year,
                PeriodMonth = effectiveDate.Month,
                FromDate = new DateTime(effectiveDate.Year, effectiveDate.Month, 1),
                ToDate = new DateTime(effectiveDate.Year, effectiveDate.Month, DateTime.DaysInMonth(effectiveDate.Year, effectiveDate.Month)),
                PlannedDisbursementDate = effectiveDate,
                Status = "Generated",
                CreatedAtUtc = DateTime.UtcNow,
                Notes = KafalaAidCycleMetadata.BuildCycleNotes(sponsorId, sponsorName, effectiveDate)
            };

            if (existingCycle == null)
            {
                _db.Set<AidCycle>().Add(cycle);
                messages.Add("تم إنشاء دورة كفالات جديدة");
            }
            else
            {
                cycle.PlannedDisbursementDate = effectiveDate;
                cycle.AidTypeId ??= sponsorshipAidTypeId;
                cycle.Notes = KafalaAidCycleMetadata.BuildCycleNotes(sponsorId, sponsorName, effectiveDate);
                cycle.UpdatedCycleTitle(sponsorName, effectiveDate, sponsorId.HasValue);
                messages.Add("تم تحديث دورة كفالات موجودة لنفس الفترة");
            }

            var existingLineNotes = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == cycle.Id)
                .Select(x => x.Notes)
                .ToListAsync();

            var existingCaseIds = existingLineNotes
                .Select(note => KafalaAidCycleMetadata.TryParseLineNotes(note, out var meta) ? meta.KafalaCaseId : null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToHashSet();

            var newLines = dueCases
                .Where(x => !existingCaseIds.Contains(x.Id))
                .Select(item => new AidCycleBeneficiary
                {
                    Id = Guid.NewGuid(),
                    AidCycleId = cycle.Id,
                    BeneficiaryId = item.BeneficiaryId,
                    AidTypeId = sponsorshipAidTypeId,
                    ScheduledAmount = item.MonthlyAmount,
                    ApprovedAmount = item.MonthlyAmount,
                    Status = "Eligible",
                    NextDueDate = item.NextDueDate ?? effectiveDate,
                    Notes = KafalaAidCycleMetadata.BuildLineNotes(item),
                    CreatedAtUtc = DateTime.UtcNow
                })
                .ToList();

            if (newLines.Count > 0)
            {
                await _db.Set<AidCycleBeneficiary>().AddRangeAsync(newLines);
            }

            await _db.SaveChangesAsync();
            await RecalculateCycleTotalsAsync(cycle.Id);

            if (newLines.Count == 0)
            {
                messages.Add("كل الكفالات المستحقة مضافة بالفعل داخل الدورة");
            }
            else
            {
                messages.Add($"تمت إضافة {newLines.Count} حالة كفالة إلى الدورة");
            }

            return (cycle.Id, newLines.Count, messages);
        }

        private async Task<Guid> EnsureSponsorshipAidTypeAsync()
        {
            var aidType = await _db.Set<AidTypeLookup>().FirstOrDefaultAsync(x => x.Id == CharityLookupSeedIds.AidTypeSponsorship)
                ?? await _db.Set<AidTypeLookup>().FirstOrDefaultAsync(x => x.NameAr == "كفالة" || x.NameEn == "Sponsorship")
                ?? await _db.Set<AidTypeLookup>().FirstOrDefaultAsync(x => x.NameAr == "مساعدة مالية");

            if (aidType != null)
                return aidType.Id;

            var entity = new AidTypeLookup
            {
                Id = CharityLookupSeedIds.AidTypeSponsorship,
                NameAr = "كفالة",
                NameEn = "Sponsorship",
                DisplayOrder = 90,
                IsActive = true
            };

            _db.Set<AidTypeLookup>().Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        private Task<AidCycle?> FindExistingCycleAsync(DateTime effectiveDate, Guid? sponsorId)
        {
            var sponsorToken = sponsorId?.ToString() ?? string.Empty;
            return _db.Set<AidCycle>()
                .FirstOrDefaultAsync(x => x.CycleType == "Sponsorship"
                                          && x.PeriodYear == effectiveDate.Year
                                          && x.PeriodMonth == effectiveDate.Month
                                          && (sponsorId == null
                                              ? x.Notes != null && x.Notes.Contains("scope=All")
                                              : x.Notes != null && x.Notes.Contains($"sponsorId={sponsorToken}")));
        }

        private async Task RecalculateCycleTotalsAsync(Guid cycleId)
        {
            var cycle = await _db.Set<AidCycle>().FirstOrDefaultAsync(x => x.Id == cycleId);
            if (cycle == null)
                return;

            var totals = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == cycleId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Planned = g.Sum(x => x.ApprovedAmount ?? x.ScheduledAmount ?? 0m),
                    Disbursed = g.Sum(x => x.DisbursedAmount ?? 0m)
                })
                .FirstOrDefaultAsync();

            cycle.BeneficiariesCount = totals?.Count ?? 0;
            cycle.TotalPlannedAmount = totals?.Planned ?? 0m;
            cycle.TotalDisbursedAmount = totals?.Disbursed ?? 0m;
            await _db.SaveChangesAsync();
        }
    }

    internal static class KafalaAidCycleBridgeExtensions
    {
        public static void UpdatedCycleTitle(this AidCycle cycle, string? sponsorName, DateTime effectiveDate, bool hasSponsorFilter)
        {
            cycle.Title = hasSponsorFilter && !string.IsNullOrWhiteSpace(sponsorName)
                ? $"دورة كفالات {effectiveDate:yyyy/MM} - {sponsorName}"
                : $"دورة كفالات {effectiveDate:yyyy/MM}";
        }
    }
}
