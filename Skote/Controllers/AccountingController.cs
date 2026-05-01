using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using Microsoft.AspNetCore.Mvc;

public class AccountingController : Controller
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICostCenterRepository _costCenterRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;

    public AccountingController(
        IAccountRepository accountRepository,
        ICostCenterRepository costCenterRepository,
        IFiscalPeriodRepository fiscalPeriodRepository)
    {
        _accountRepository = accountRepository;
        _costCenterRepository = costCenterRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.AccountsCount = (await _accountRepository.GetAllAsync()).Count;
        ViewBag.CostCentersCount = (await _costCenterRepository.GetAllAsync()).Count;
        ViewBag.CurrentFiscalPeriod = await _fiscalPeriodRepository.GetCurrentAsync();
        return View();
    }
}
