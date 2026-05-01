using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentWebFramework.Models.AccountingReports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AccountingReportsController : Controller
{
    private readonly IAccountingReportRepository _accountingReportRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly ICostCenterRepository _costCenterRepository;

    public AccountingReportsController(
        IAccountingReportRepository accountingReportRepository,
        IAccountRepository accountRepository,
        IFiscalPeriodRepository fiscalPeriodRepository,
        ICostCenterRepository costCenterRepository)
    {
        _accountingReportRepository = accountingReportRepository;
        _accountRepository = accountRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _costCenterRepository = costCenterRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TrialBalance(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new TrialBalancePageVm
        {
            Filter = new AccountingReportFilterVm
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);
        vm.Rows = await _accountingReportRepository.GetTrialBalanceAsync(fiscalPeriodId, fromDate, toDate);

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> GeneralLedger(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new GeneralLedgerPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);
        vm.Rows = await _accountingReportRepository.GetGeneralLedgerAsync(fiscalPeriodId, fromDate, toDate);

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> AccountStatement(Guid? accountId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new AccountStatementPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                AccountId = accountId,
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);

        if (accountId.HasValue)
        {
            vm.Statement = await _accountingReportRepository.GetAccountStatementAsync(accountId.Value, fiscalPeriodId, fromDate, toDate);
            if (vm.Statement == null)
            {
                TempData["Error"] = "الحساب المطلوب غير موجود";
            }
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> CostCenterStatement(Guid? costCenterId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new CostCenterStatementPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                CostCenterId = costCenterId,
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);

        if (costCenterId.HasValue)
        {
            vm.Statement = await _accountingReportRepository.GetCostCenterStatementAsync(costCenterId.Value, fiscalPeriodId, fromDate, toDate);
            if (vm.Statement == null)
            {
                TempData["Error"] = "مركز التكلفة المطلوب غير موجود";
            }
        }

        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> IncomeStatement(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new IncomeStatementPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);
        vm.Result = await _accountingReportRepository.GetIncomeStatementAsync(fiscalPeriodId, fromDate, toDate);

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> BalanceSheet(Guid? fiscalPeriodId, DateTime? asOfDate)
    {
        var vm = new BalanceSheetPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                FiscalPeriodId = fiscalPeriodId,
                ToDate = asOfDate
            }
        };

        await FillSharedLookups(vm.Filter);
        vm.Result = await _accountingReportRepository.GetBalanceSheetAsync(fiscalPeriodId, asOfDate);

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> RevenueExpenseSummary(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new RevenueExpenseSummaryPageVm
        {
            Filter = new AccountingReportFilterVm
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            }
        };

        await FillSharedLookups(vm.Filter);
        vm.Result = await _accountingReportRepository.GetRevenueExpenseSummaryAsync(fiscalPeriodId, fromDate, toDate);

        return View(vm);
    }

    private async Task FillSharedLookups(AccountingReportFilterVm vm)
    {
        var periods = await _fiscalPeriodRepository.GetAllAsync();
        vm.FiscalPeriods = periods
            .OrderByDescending(x => x.StartDate)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.PeriodCode} - {x.PeriodNameAr}"
            })
            .ToList();

        var accounts = await _accountRepository.GetAllAsync();
        vm.Accounts = accounts
            .Where(x => x.IsPosting && x.IsActive)
            .OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToList();

        var costCenters = await _costCenterRepository.GetAllAsync();
        vm.CostCenters = costCenters
            .Where(x => x.IsActive)
            .OrderBy(x => x.CostCenterCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.CostCenterCode} - {x.NameAr}"
            })
            .ToList();
    }
}
