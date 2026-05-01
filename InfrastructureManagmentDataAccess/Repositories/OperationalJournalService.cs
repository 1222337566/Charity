using System.Reflection;
using System.Text.RegularExpressions;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class OperationalJournalService : IOperationalJournalService
    {
        private readonly AppDbContext _db;
        private readonly IAccountingIntegrationProfileRepository _profileRepository;
        private readonly IProjectAccountingProfileRepository _projectAccountingProfileRepository;
        private readonly IProjectExpenseLinkRepository _projectExpenseLinkRepository;

        public OperationalJournalService(
            AppDbContext db,
            IAccountingIntegrationProfileRepository profileRepository,
            IProjectAccountingProfileRepository projectAccountingProfileRepository,
            IProjectExpenseLinkRepository projectExpenseLinkRepository)
        {
            _db = db;
            _profileRepository = profileRepository;
            _projectAccountingProfileRepository = projectAccountingProfileRepository;
            _projectExpenseLinkRepository = projectExpenseLinkRepository;
        }

        public async Task<JournalEntry> CreateDonationEntryAsync(Guid donationId)
        {
            var donation = await _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.Donor)
                .FirstOrDefaultAsync(x => x.Id == donationId);
            if (donation == null) throw new InvalidOperationException("لم يتم العثور على التبرع المطلوب");
            if (!donation.Amount.HasValue || donation.Amount.Value <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد لتبرع بدون مبلغ نقدي");

            var profile = await GetDonationProfileAsync(donation);
            var debitAccount = await ResolveAccountAsync(profile, donation.FinancialAccountId, true, "طرف المدين في التبرع");
            var creditAccount = await ResolveAccountAsync(profile, donation.FinancialAccountId, false, "طرف الدائن في التبرع");
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.Donation, donation.Id, donation.DonationDate,
                $"إثبات تبرع رقم {donation.DonationNumber} - {donation.Donor?.FullName}",
                debitAccount.Id, creditAccount.Id, donation.Amount.Value, profile.DefaultCostCenterId, null, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateExpenseEntryAsync(Guid expenseId)
        {
            var expense = await _db.Set<Expensex>()
                .AsNoTracking()
                .Include(x => x.ExpenseCategory)
                .FirstOrDefaultAsync(x => x.Id == expenseId);
            if (expense == null) throw new InvalidOperationException("لم يتم العثور على المصروف المطلوب");
            if (expense.Amount <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد لمصروف بقيمة صفرية");

            var profile = await GetProfileAsync(AccountingSourceTypes.Expense);
            var debitAccount = await ResolveAccountAsync(profile, null, true, "حساب مدين المصروف");
            var creditAccount = await ResolveAccountAsync(profile, null, false, "حساب دائن المصروف");

            var link = await _projectExpenseLinkRepository.GetByExpenseIdAsync(expenseId);
            var projectId = link?.ProjectId;
            var costCenterId = link?.CostCenterId;
            if (!costCenterId.HasValue && projectId.HasValue)
            {
                var projectProfile = await _projectAccountingProfileRepository.GetByProjectIdAsync(projectId.Value);
                if (projectProfile?.AutoUseProjectCostCenter == true)
                    costCenterId = projectProfile.DefaultCostCenterId;
            }
            costCenterId ??= profile.DefaultCostCenterId;
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.Expense, expense.Id, expense.ExpenseDateUtc,
                $"إثبات مصروف رقم {expense.ExpenseNumber} - {expense.ExpenseCategory?.NameAr}",
                debitAccount.Id, creditAccount.Id, expense.Amount, costCenterId, projectId, profile.AutoPost);
        }

        public async Task<JournalEntry> CreatePayrollEntryAsync(Guid payrollMonthId)
        {
            var payrollMonth = await _db.Set<PayrollMonth>()
                .Include(x => x.Employees)
                .FirstOrDefaultAsync(x => x.Id == payrollMonthId);
            if (payrollMonth == null) throw new InvalidOperationException("لم يتم العثور على شهر المرتبات المطلوب");

            var amount = payrollMonth.Employees.Sum(x => x.NetAmount);
            if (amount <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد مرتبات قبل اعتماد صافي مستحقات الموظفين");

            var profile = await GetProfileAsync(AccountingSourceTypes.Payroll);
            var debitAccount = await ResolveAccountAsync(profile, null, true, "حساب مدين المرتبات");
            var creditAccount = await ResolveAccountAsync(profile, null, false, "حساب دائن المرتبات");
            var entryDate = new DateTime(payrollMonth.Year, payrollMonth.Month, DateTime.DaysInMonth(payrollMonth.Year, payrollMonth.Month));
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.Payroll, payrollMonth.Id, entryDate,
                $"إثبات رواتب شهر {payrollMonth.Month}/{payrollMonth.Year}",
                debitAccount.Id, creditAccount.Id, amount, profile.DefaultCostCenterId, null, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateStoreIssueEntryAsync(Guid storeIssueId)
        {
            var issue = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == storeIssueId);
            if (issue == null) throw new InvalidOperationException("لم يتم العثور على إذن الصرف المخزني المطلوب");

            var amount = issue.Lines.Sum(x => x.Quantity * x.UnitCost);
            if (amount <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد لصرف مخزني بدون قيمة");

            var profile = await GetStoreIssueProfileAsync(issue);
            var debitAccount = await ResolveAccountAsync(profile, null, true, "حساب مدين الصرف المخزني");
            var creditAccount = await ResolveAccountAsync(profile, null, false, "حساب دائن الصرف المخزني");

            Guid? costCenterId = profile.DefaultCostCenterId;
            if (issue.ProjectId.HasValue)
            {
                var projectProfile = await _projectAccountingProfileRepository.GetByProjectIdAsync(issue.ProjectId.Value);
                if (projectProfile?.AutoUseProjectCostCenter == true)
                    costCenterId = projectProfile.DefaultCostCenterId ?? costCenterId;
            }
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.StoreIssue, issue.Id, issue.IssueDate,
                $"إثبات صرف مخزني رقم {issue.IssueNumber} من مخزن {issue.Warehouse?.WarehouseNameAr}",
                debitAccount.Id, creditAccount.Id,
                amount, costCenterId, issue.ProjectId, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateStoreReceiptEntryAsync(Guid storeReceiptId)
        {
            var receipt = await _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == storeReceiptId);
            if (receipt == null) throw new InvalidOperationException("لم يتم العثور على إذن الإضافة المخزنية المطلوب");

            var amount = receipt.Lines.Sum(x => x.Quantity * x.UnitCost);
            if (amount <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد لإذن إضافة بدون قيمة");

            var profile = await GetStoreReceiptProfileAsync(receipt);
            var debitAccount = await ResolveAccountAsync(profile, null, true, "حساب مدين الإضافة المخزنية");
            var creditAccount = await ResolveAccountAsync(profile, null, false, "حساب دائن الإضافة المخزنية");
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.StoreReceipt, receipt.Id, receipt.ReceiptDate,
                $"إثبات إضافة مخزنية رقم {receipt.ReceiptNumber}", debitAccount.Id, creditAccount.Id,
                amount, profile.DefaultCostCenterId, null, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateGrantInstallmentEntryAsync(Guid grantInstallmentId)
        {
            var installment = await _db.Set<GrantInstallment>()
                .AsNoTracking()
                .Include(x => x.GrantAgreement)
                .FirstOrDefaultAsync(x => x.Id == grantInstallmentId);
            if (installment == null) throw new InvalidOperationException("لم يتم العثور على دفعة التمويل المطلوبة");
            if (!installment.ReceivedAmount.HasValue || installment.ReceivedAmount.Value <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد قبل استلام قيمة دفعة التمويل");

            var profile = await GetProfileAsync(AccountingSourceTypes.GrantInstallment);
            var debitAccount = await ResolveAccountAsync(profile, installment.FinancialAccountId, true, "حساب مدين دفعة التمويل");
            var creditAccount = await ResolveAccountAsync(profile, installment.FinancialAccountId, false, "حساب دائن دفعة التمويل");

            Guid? projectId = await _db.Set<ProjectGrant>()
                .Where(x => x.GrantAgreementId == installment.GrantAgreementId)
                .Select(x => (Guid?)x.ProjectId)
                .FirstOrDefaultAsync();

            Guid? costCenterId = profile.DefaultCostCenterId;
            if (projectId.HasValue)
            {
                var projectProfile = await _projectAccountingProfileRepository.GetByProjectIdAsync(projectId.Value);
                if (projectProfile?.AutoUseProjectCostCenter == true)
                    costCenterId = projectProfile.DefaultCostCenterId ?? costCenterId;
            }
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.GrantInstallment, installment.Id,
                installment.ReceivedDate ?? installment.DueDate,
                $"إثبات استلام دفعة تمويل رقم {installment.InstallmentNumber} من اتفاقية {installment.GrantAgreement?.AgreementNumber}",
                debitAccount.Id, creditAccount.Id, installment.ReceivedAmount.Value, costCenterId, projectId, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateBeneficiaryAidDisbursementEntryAsync(Guid aidDisbursementId)
        {
            var aidDisbursement = await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.AidRequest)
                .FirstOrDefaultAsync(x => x.Id == aidDisbursementId);
            if (aidDisbursement == null) throw new InvalidOperationException("لم يتم العثور على عملية صرف المساعدة المطلوبة");
            if (!aidDisbursement.Amount.HasValue || aidDisbursement.Amount.Value <= 0) throw new InvalidOperationException("لا يمكن إنشاء قيد لصرف مساعدة بدون مبلغ");

            var profile = await GetBeneficiaryAidDisbursementProfileAsync(aidDisbursement);
            var debitAccount = await ResolveAccountAsync(profile, aidDisbursement.FinancialAccountId, true, "حساب مدين صرف المساعدة");
            var creditAccount = await ResolveAccountAsync(profile, aidDisbursement.FinancialAccountId, false, "حساب دائن صرف المساعدة");

            Guid? projectId = aidDisbursement.ProjectId ?? aidDisbursement.AidRequest?.ProjectId;
            if (!projectId.HasValue && aidDisbursement.GrantAgreementId.HasValue)
            {
                projectId = await _db.Set<ProjectGrant>()
                    .Where(x => x.GrantAgreementId == aidDisbursement.GrantAgreementId.Value)
                    .Select(x => (Guid?)x.ProjectId)
                    .FirstOrDefaultAsync();
            }

            Guid? costCenterId = profile.DefaultCostCenterId;
            if (projectId.HasValue)
            {
                var projectProfile = await _projectAccountingProfileRepository.GetByProjectIdAsync(projectId.Value);
                if (projectProfile?.AutoUseProjectCostCenter == true)
                    costCenterId = projectProfile.DefaultCostCenterId ?? costCenterId;
            }
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(AccountingSourceTypes.BeneficiaryAidDisbursement, aidDisbursement.Id,
                aidDisbursement.DisbursementDate,
                $"إثبات صرف مساعدة للمستفيد {aidDisbursement.Beneficiary?.FullName} - {aidDisbursement.AidType?.NameAr}",
                debitAccount.Id, creditAccount.Id, aidDisbursement.Amount.Value, costCenterId, projectId, profile.AutoPost);
        }

        public async Task<JournalEntry> CreateDynamicEntryAsync(string sourceType, Guid sourceId)
        {
            sourceType = Normalize(sourceType);
            if (string.IsNullOrWhiteSpace(sourceType))
                throw new InvalidOperationException("نوع المصدر مطلوب لإنشاء قيد ديناميكي.");

            var definition = await _db.Set<AccountingIntegrationSourceDefinition>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SourceType == sourceType && x.IsActive);

            if (definition == null)
                throw new InvalidOperationException($"مصدر الربط المحاسبي {sourceType} غير معرف أو غير نشط.");

            if (!definition.IsDynamicPostingEnabled)
                throw new InvalidOperationException($"مصدر الربط المحاسبي {sourceType} غير مفعل للتوليد الديناميكي.");

            var entityType = ResolveEntityType(definition.EntityClrTypeName);
            if (entityType == null)
                throw new InvalidOperationException($"لم يتم العثور على كلاس الحركة: {definition.EntityClrTypeName}.");

            var source = await _db.FindAsync(entityType, sourceId);
            if (source == null)
                throw new InvalidOperationException("لم يتم العثور على حركة التشغيل المطلوبة.");

            var amount = ReadDecimal(source, definition.AmountPropertyName, "Amount", "TotalAmount", "NetAmount", "ReceivedAmount", "ApprovedAmount", "Value");
            if (!amount.HasValue || amount.Value <= 0)
                throw new InvalidOperationException("لا يمكن إنشاء قيد ديناميكي بدون قيمة موجبة. راجع حقل القيمة في خريطة المصدر.");

            var entryDate = ReadDate(source, definition.DatePropertyName, "DocumentDate", "EntryDate", "TransactionDate", "OperationDate", "PaymentDate", "ReceivedDate", "DueDate", "CreatedAtUtc")
                ?? DateTime.Today;

            var sourceFinancialAccountId = ReadGuid(source, definition.FinancialAccountIdPropertyName, "FinancialAccountId", "CashAccountId", "BankAccountId");
            var projectId = ReadGuid(source, definition.ProjectIdPropertyName, "ProjectId");
            var costCenterId = ReadGuid(source, definition.CostCenterIdPropertyName, "CostCenterId");
            var eventCode = ReadString(source, definition.EventCodePropertyName, "EventCode", "PostingEventCode", "MovementType");

            var profile = await GetDynamicProfileAsync(definition, source, eventCode);
            var debitAccount = await ResolveAccountAsync(profile, sourceFinancialAccountId, true, $"حساب مدين المصدر {definition.NameAr}");
            var creditAccount = await ResolveAccountAsync(profile, sourceFinancialAccountId, false, $"حساب دائن المصدر {definition.NameAr}");

            costCenterId ??= profile.DefaultCostCenterId;
            if (projectId.HasValue)
            {
                var projectProfile = await _projectAccountingProfileRepository.GetByProjectIdAsync(projectId.Value);
                if (projectProfile?.AutoUseProjectCostCenter == true)
                    costCenterId = projectProfile.DefaultCostCenterId ?? costCenterId;
            }

            var description = BuildDynamicDescription(definition, source, amount.Value);
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(sourceType, sourceId, entryDate, description,
                debitAccount.Id, creditAccount.Id, amount.Value, costCenterId, projectId, profile.AutoPost);
        }

        private async Task<AccountingIntegrationProfile> GetProfileAsync(string sourceType)
        {
            var profiles = await _profileRepository.GetActiveBySourceTypeAsync(sourceType);
            var profile = SelectBestProfile(profiles, null, null, null, null, null, null);
            if (profile == null) throw new InvalidOperationException($"لا توجد Profile محاسبية مفعلة لنوع المصدر {sourceType}");
            return profile;
        }

        private async Task<AccountingIntegrationProfile> GetDonationProfileAsync(Donation donation)
        {
            var profiles = await _profileRepository.GetActiveBySourceTypeAsync(AccountingSourceTypes.Donation);
            var profile = SelectBestProfile(
                profiles,
                eventCode: null,
                donationType: donation.DonationType,
                targetingScopeCode: donation.TargetingScopeCode,
                purposeName: donation.GeneralPurposeName,
                aidTypeId: donation.AidTypeId,
                storeMovementType: null);

            if (profile == null)
                throw new InvalidOperationException("لا توجد Profile محاسبية نشطة تطابق بيانات التبرع. راجع مصدر التبرع ونوعه ونطاق التوجيه داخل Profiles الربط المحاسبي.");

            return profile;
        }

        private async Task<AccountingIntegrationProfile> GetBeneficiaryAidDisbursementProfileAsync(BeneficiaryAidDisbursement aidDisbursement)
        {
            var profiles = await _profileRepository.GetActiveBySourceTypeAsync(AccountingSourceTypes.BeneficiaryAidDisbursement);
            var profile = SelectBestProfile(
                profiles,
                eventCode: null,
                donationType: null,
                targetingScopeCode: null,
                purposeName: null,
                aidTypeId: aidDisbursement.AidTypeId,
                storeMovementType: null);

            if (profile == null)
                throw new InvalidOperationException("لا توجد Profile محاسبية نشطة تطابق صرف المساعدة. راجع نوع المساعدة داخل Profile الربط المحاسبي.");

            return profile;
        }

        private async Task<AccountingIntegrationProfile> GetStoreIssueProfileAsync(CharityStoreIssue issue)
        {
            var profiles = await _profileRepository.GetActiveBySourceTypeAsync(AccountingSourceTypes.StoreIssue);
            var profile = SelectBestProfile(
                profiles,
                eventCode: null,
                donationType: null,
                targetingScopeCode: null,
                purposeName: null,
                aidTypeId: null,
                storeMovementType: issue.IssueType);

            if (profile == null)
                throw new InvalidOperationException("لا توجد Profile محاسبية نشطة تطابق نوع الصرف المخزني.");

            return profile;
        }

        private async Task<AccountingIntegrationProfile> GetStoreReceiptProfileAsync(CharityStoreReceipt receipt)
        {
            var profiles = await _profileRepository.GetActiveBySourceTypeAsync(AccountingSourceTypes.StoreReceipt);
            var profile = SelectBestProfile(
                profiles,
                eventCode: null,
                donationType: null,
                targetingScopeCode: null,
                purposeName: null,
                aidTypeId: null,
                storeMovementType: receipt.SourceType);

            if (profile == null)
                throw new InvalidOperationException("لا توجد Profile محاسبية نشطة تطابق نوع الإضافة المخزنية.");

            return profile;
        }

        private async Task<AccountingIntegrationProfile> GetDynamicProfileAsync(AccountingIntegrationSourceDefinition definition, object source, string? eventCode)
        {
            // جرّب البحث بـ SourceType المحدد + بديل case-insensitive من DB
            var allActiveProfiles = await _db.Set<AccountingIntegrationProfile>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Priority)
                .ToListAsync();

            var profiles = allActiveProfiles
                .Where(x => string.Equals(x.SourceType, definition.SourceType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var profile = SelectBestProfile(
                profiles,
                eventCode: eventCode,
                donationType: ReadString(source, definition.DonationTypePropertyName, "DonationType"),
                targetingScopeCode: ReadString(source, definition.TargetingScopeCodePropertyName, "TargetingScopeCode"),
                purposeName: ReadString(source, definition.PurposeNamePropertyName, "PurposeName", "GeneralPurposeName"),
                aidTypeId: ReadGuid(source, definition.AidTypeIdPropertyName, "AidTypeId"),
                storeMovementType: ReadString(source, definition.StoreMovementTypePropertyName, "StoreMovementType", "IssueType", "SourceType"));

            // لو ما لقى — جرّب بدون شروط مطابقة (General fallback)
            if (profile == null && profiles.Any())
                profile = profiles.OrderBy(x => x.Priority).First();

            if (profile == null)
                throw new InvalidOperationException(
                    $"لا توجد Profile محاسبية نشطة تطابق المصدر الديناميكي "+ definition.SourceType +". " +
                    $"أضف Profile من صفحة الربط المحاسبي بنفس الكود.");

            return profile;
        }

        private static AccountingIntegrationProfile? SelectBestProfile(
            List<AccountingIntegrationProfile> profiles,
            string? eventCode,
            string? donationType,
            string? targetingScopeCode,
            string? purposeName,
            Guid? aidTypeId,
            string? storeMovementType)
        {
            return profiles
                .Where(x => string.IsNullOrWhiteSpace(eventCode) || MatchesText(x.EventCode, eventCode))
                .Where(x => MatchesText(x.MatchDonationType, donationType))
                .Where(x => MatchesText(x.MatchTargetingScopeCode, targetingScopeCode))
                .Where(x => MatchesPurpose(x.MatchPurposeName, purposeName))
                .Where(x => !x.MatchAidTypeId.HasValue || (aidTypeId.HasValue && x.MatchAidTypeId.Value == aidTypeId.Value))
                .Where(x => MatchesText(x.MatchStoreMovementType, storeMovementType))
                .OrderByDescending(GetSpecificityScore)
                .ThenBy(x => x.Priority)
                .ThenBy(x => x.ProfileNameAr)
                .FirstOrDefault();
        }

        private static int GetSpecificityScore(AccountingIntegrationProfile profile)
        {
            var score = 0;
            if (!string.IsNullOrWhiteSpace(profile.EventCode)) score++;
            if (!string.IsNullOrWhiteSpace(profile.MatchDonationType)) score++;
            if (!string.IsNullOrWhiteSpace(profile.MatchTargetingScopeCode)) score++;
            if (!string.IsNullOrWhiteSpace(profile.MatchPurposeName)) score++;
            if (profile.MatchAidTypeId.HasValue) score++;
            if (!string.IsNullOrWhiteSpace(profile.MatchStoreMovementType)) score++;
            return score;
        }

        private static bool MatchesText(string? configuredValue, string? actualValue)
        {
            if (string.IsNullOrWhiteSpace(configuredValue))
                return true;

            return string.Equals(configuredValue.Trim(), actualValue?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesPurpose(string? configuredPurpose, string? actualPurpose)
        {
            if (string.IsNullOrWhiteSpace(configuredPurpose))
                return true;

            var actual = actualPurpose?.Trim();
            return !string.IsNullOrWhiteSpace(actual)
                && actual.Contains(configuredPurpose.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private async Task<FinancialAccount> ResolveAccountAsync(AccountingIntegrationProfile profile, Guid? sourceFinancialAccountId, bool isDebit, string label)
        {
            Guid? accountId = isDebit
                ? (profile.UseSourceFinancialAccountAsDebit ? sourceFinancialAccountId : profile.DebitAccountId)
                : (profile.UseSourceFinancialAccountAsCredit ? sourceFinancialAccountId : profile.CreditAccountId);

            if (!accountId.HasValue) throw new InvalidOperationException($"لم يتم تحديد {label} داخل Profile الربط المحاسبي");
            var account = await _db.Set<FinancialAccount>().FirstOrDefaultAsync(x => x.Id == accountId.Value);
            if (account == null || !account.IsActive || !account.IsPosting) throw new InvalidOperationException($"الحساب المختار لـ {label} غير صالح للترحيل");
            return account;
        }

        private async Task<JournalEntry> CreateTwoLineEntryAsync(string sourceType, Guid sourceId, DateTime entryDate, string description,
            Guid debitAccountId, Guid creditAccountId, decimal amount, Guid? costCenterId, Guid? projectId, bool autoPost)
        {
            var existing = await _db.Set<JournalEntry>()
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.SourceType == sourceType && x.SourceId == sourceId);
            if (existing != null)
                return existing;

            var fiscalPeriod = await _db.Set<FiscalPeriod>()
                .Where(x => x.IsActive && x.IsOpen && x.StartDate <= entryDate.Date && x.EndDate >= entryDate.Date)
                .OrderBy(x => x.StartDate)
                .FirstOrDefaultAsync();
            if (fiscalPeriod == null) throw new InvalidOperationException("لا توجد فترة مالية مفتوحة لتاريخ القيد");

            var count = await _db.Set<JournalEntry>().CountAsync(x => x.EntryDate.Year == entryDate.Year);
            var entry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryNumber = $"JV-{entryDate.Year}-{(count + 1).ToString("D5")}",
                EntryDate = entryDate,
                Description = description,
                FiscalPeriodId = fiscalPeriod.Id,
                Status = autoPost ? JournalEntryStatus.Posted : JournalEntryStatus.Draft,
                SourceType = sourceType,
                SourceId = sourceId,
                CreatedAtUtc = DateTime.UtcNow,
                PostedAtUtc = autoPost ? DateTime.UtcNow : null,
                Lines = new List<JournalEntryLine>
                {
                    new JournalEntryLine
                    {
                        Id = Guid.NewGuid(), JournalEntryId = Guid.Empty, FinancialAccountId = debitAccountId,
                        CostCenterId = costCenterId, ProjectId = projectId, DebitAmount = amount, CreditAmount = 0m,
                        Description = description, CreatedAtUtc = DateTime.UtcNow
                    },
                    new JournalEntryLine
                    {
                        Id = Guid.NewGuid(), JournalEntryId = Guid.Empty, FinancialAccountId = creditAccountId,
                        CostCenterId = costCenterId, ProjectId = projectId, DebitAmount = 0m, CreditAmount = amount,
                        Description = description, CreatedAtUtc = DateTime.UtcNow
                    }
                }
            };
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            foreach (var line in entry.Lines) line.JournalEntryId = entry.Id;
            _db.Set<JournalEntry>().Add(entry);
            await _db.SaveChangesAsync();
            return entry;
        }

        private static Type? ResolveEntityType(string? entityClrTypeName)
        {
            var wanted = Normalize(entityClrTypeName);
            if (string.IsNullOrWhiteSpace(wanted))
                return null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(x => x != null).Cast<Type>().ToArray();
                }

                var match = types.FirstOrDefault(x =>
                    x.IsClass &&
                    (string.Equals(x.FullName, wanted, StringComparison.Ordinal)
                     || string.Equals(x.Name, wanted, StringComparison.OrdinalIgnoreCase)));

                if (match != null)
                    return match;
            }

            return Type.GetType(wanted, throwOnError: false, ignoreCase: true);
        }

        private static string BuildDynamicDescription(AccountingIntegrationSourceDefinition definition, object source, decimal amount)
        {
            if (!string.IsNullOrWhiteSpace(definition.DescriptionTemplate))
                return ApplyTemplate(definition.DescriptionTemplate!, source, amount);

            var number = ReadString(source, definition.NumberPropertyName, "InvoiceNumber", "DonationNumber", "IssueNumber", "ReceiptNumber", "DocumentNumber", "Number", "Code");
            var title = ReadString(source, definition.TitlePropertyName, "SupplierName", "Title", "Name", "NameAr", "FullName", "Notes");
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(definition.NameAr)) parts.Add(definition.NameAr);
            if (!string.IsNullOrWhiteSpace(number)) parts.Add($"رقم: {number}");
            if (!string.IsNullOrWhiteSpace(title)) parts.Add(title);
            return string.Join(" | ", parts);
        }

        private static string ApplyTemplate(string template, object source, decimal amount)
        {
            return Regex.Replace(template, "\\{(?<name>[A-Za-z0-9_]+)\\}", match =>
            {
                var name = match.Groups["n"].Value;
                if (string.Equals(name, "Amount", StringComparison.OrdinalIgnoreCase))
                    return amount.ToString("0.##");

                var value = ReadRawValue(source, name);
                return FormatValue(value) ?? string.Empty;
            });
        }
        public async Task<JournalEntry> CreateSalesInvoiceCogsEntryAsync(Guid salesInvoiceId)
        {
            var invoice = await _db.Set<SalesInvoice>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == salesInvoiceId);

            if (invoice == null)
                throw new InvalidOperationException("لم يتم العثور على فاتورة البيع المطلوبة.");

            // Patch 35C creates a StoreIssue linked to the SalesInvoice.
            // It should have SourceType = SalesInvoice and SourceId = invoice.Id.
            var issue = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x =>
                    x.SourceType == AccountingSourceTypes.SalesInvoice &&
                    x.SourceId == salesInvoiceId);

            if (issue == null)
                throw new InvalidOperationException("لم يتم العثور على إذن صرف مخزني مرتبط بفاتورة البيع.");

            var amount = issue.Lines.Sum(x => x.Quantity * x.UnitCost);

            if (amount <= 0)
                throw new InvalidOperationException("لا يمكن إنشاء قيد تكلفة مبيعات بقيمة صفرية. راجع UnitCost في إذن الصرف وحركات المخزون.");

            var profile = await GetProfileAsync(AccountingSourceTypes.SalesInvoiceCOGS);
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            // Debit: COGS expense account
            // Credit: Inventory account
            var debitAccount = await ResolveAccountAsync(profile, null, true, "حساب تكلفة البضاعة المباعة");
            var creditAccount = await ResolveAccountAsync(profile, null, false, "حساب المخزون");
            await EnsurePeriodIsOpenAsync(DateTime.Now);
            return await CreateTwoLineEntryAsync(
                AccountingSourceTypes.SalesInvoiceCOGS,
                invoice.Id,
                invoice.InvoiceDateUtc,
                $"إثبات تكلفة البضاعة المباعة لفاتورة بيع رقم {invoice.InvoiceNumber}",
                debitAccount.Id,
                creditAccount.Id,
                amount,
                profile.DefaultCostCenterId,
                null,
                profile.AutoPost);
        }

        private static string? ReadString(object source, string? configuredName, params string[] fallbackNames)
        {
            var value = ReadFirstValue(source, configuredName, fallbackNames);
            return FormatValue(value);
        }

        private static decimal? ReadDecimal(object source, string? configuredName, params string[] fallbackNames)
        {
            var value = ReadFirstValue(source, configuredName, fallbackNames);
            if (value == null)
                return null;

            if (value is decimal d) return d;
            if (value is int i) return i;
            if (value is long l) return l;
            if (value is double db) return Convert.ToDecimal(db);
            if (value is float f) return Convert.ToDecimal(f);
            if (decimal.TryParse(value.ToString(), out var parsed)) return parsed;
            return null;
        }

        private static DateTime? ReadDate(object source, string? configuredName, params string[] fallbackNames)
        {
            var value = ReadFirstValue(source, configuredName, fallbackNames);
            if (value == null)
                return null;

            if (value is DateTime dt) return dt;
            if (DateTime.TryParse(value.ToString(), out var parsed)) return parsed;
            return null;
        }

        private static Guid? ReadGuid(object source, string? configuredName, params string[] fallbackNames)
        {
            var value = ReadFirstValue(source, configuredName, fallbackNames);
            if (value == null)
                return null;

            if (value is Guid guid && guid != Guid.Empty) return guid;
            if (Guid.TryParse(value.ToString(), out var parsed) && parsed != Guid.Empty) return parsed;
            return null;
        }

        private static object? ReadFirstValue(object source, string? configuredName, params string[] fallbackNames)
        {
            if (!string.IsNullOrWhiteSpace(configuredName))
            {
                var configuredValue = ReadRawValue(source, configuredName!);
                if (configuredValue != null)
                    return configuredValue;
            }

            foreach (var fallback in fallbackNames)
            {
                var value = ReadRawValue(source, fallback);
                if (value != null)
                    return value;
            }

            return null;
        }

        private static object? ReadRawValue(object source, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            var prop = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => string.Equals(x.Name, propertyName.Trim(), StringComparison.OrdinalIgnoreCase));

            return prop?.CanRead == true ? prop.GetValue(source) : null;
        }

        private static string? FormatValue(object? value)
        {
            return value switch
            {
                null => null,
                DateTime dt => dt.ToString("yyyy-MM-dd"),
                Guid guid => guid == Guid.Empty ? null : guid.ToString(),
                decimal d => d.ToString("0.##"),
                _ => string.IsNullOrWhiteSpace(value.ToString()) ? null : value.ToString()
            };
        }
        private async Task EnsurePeriodIsOpenAsync(DateTime operationDateUtc)
        {
            var closedPeriod = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsClosed &&
                    operationDateUtc >= x.StartDate &&
                    operationDateUtc <= x.EndDate);

            if (closedPeriod != null)
                throw new InvalidOperationException($"الفترة المالية ({closedPeriod.PeriodNameAr}) مقفلة ولا يمكن إنشاء قيود بتاريخ داخلها.");
        }
        private static string Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}
