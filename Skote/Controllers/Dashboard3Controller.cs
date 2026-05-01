using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using InfrastructureManagmentWebFramework.Models.Dashboard;
using InfrastructureManagmentWebFramework.Models.Reports.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    [Authorize]
    public class Dashboard3Controller : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ISalesInvoiceRepository _salesInvoiceRepository;
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
        private readonly IPosHoldRepository _posHoldRepository;
        private readonly IItemWarehouseBalanceRepository _itemWarehouseBalanceRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public Dashboard3Controller(
            IItemRepository itemRepository,
            IWarehouseRepository warehouseRepository,
            ISalesInvoiceRepository salesInvoiceRepository,
            IPurchaseInvoiceRepository purchaseInvoiceRepository,
            IPosHoldRepository posHoldRepository,
            IItemWarehouseBalanceRepository itemWarehouseBalanceRepository,
            IPaymentMethodRepository paymentMethodRepository)
        {
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _posHoldRepository = posHoldRepository;
            _itemWarehouseBalanceRepository = itemWarehouseBalanceRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "لوحة التحكم";
            ViewBag.pTitle = "لوحة التحكم";
            ViewBag.pageTitle = "الرئيسية";

            var dashboardRepository = HttpContext.RequestServices.GetService(typeof(ICharityDashboardRepository)) as ICharityDashboardRepository;
            if (dashboardRepository == null)
            {
                return RedirectToAction(nameof(DashboardAccounts));
            }

            var snapshot = await dashboardRepository.GetSnapshotAsync();
            if (snapshot == null)
            {
                return View(CreateEmptyCharityDashboardVm());
            }

            var model = new CharityDashboardVm
            {
                BeneficiariesCount = snapshot.BeneficiariesCount,
                DonorsCount = snapshot.DonorsCount,
                FundersCount = snapshot.FundersCount,
                ProjectsCount = snapshot.ProjectsCount,
                EmployeesCount = snapshot.EmployeesCount,
                TotalDonations = snapshot.TotalDonations,
                TotalReceivedGrants = snapshot.TotalReceivedGrants,
                TotalAidDisbursed = snapshot.TotalAidDisbursed,
                TotalPayrollNet = snapshot.TotalPayrollNet,
                BeneficiaryStatuses = snapshot.BeneficiaryStatuses ?? new(),
                MonthlyDonations = snapshot.MonthlyDonations ?? new(),
                TopProjects = snapshot.TopProjects ?? new(),
                RecentPayrollMonths = snapshot.RecentPayrollMonths ?? new(),
                StoreMovements = snapshot.StoreMovements ?? new(),
                // Kafala
                KafalaCasesTotal      = snapshot.KafalaCasesTotal,
                KafalaCasesActive     = snapshot.KafalaCasesActive,
                KafalaCasesSuspended  = snapshot.KafalaCasesSuspended,
                KafalaCasesClosed     = snapshot.KafalaCasesClosed,
                KafalaSponsorsCount   = snapshot.KafalaSponsorsCount,
                KafalaTotalCollected  = snapshot.KafalaTotalCollected,
                KafalaTotalDisbursed  = snapshot.KafalaTotalDisbursed,
                KafalaBySponsorshipType = snapshot.KafalaBySponsorshipType ?? new(),
                KafalaByFrequency     = snapshot.KafalaByFrequency ?? new(),
                KafalaMonthlyCollected  = snapshot.KafalaMonthlyCollected  ?? new(),
                KafalaMonthlyDisbursed  = snapshot.KafalaMonthlyDisbursed  ?? new(),
                // Accounting
                TotalAssets      = snapshot.TotalAssets,
                TotalLiabilities = snapshot.TotalLiabilities,
                TotalEquity      = snapshot.TotalEquity,
                TotalRevenue     = snapshot.TotalRevenue,
                TotalExpenses    = snapshot.TotalExpenses,
                AccountsByCategory = snapshot.AccountsByCategory ?? new(),
                MonthlyRevenue     = snapshot.MonthlyRevenue   ?? new(),
                MonthlyExpenses    = snapshot.MonthlyExpenses  ?? new(),
                // Historical
                MonthlyNewBeneficiaries  = snapshot.MonthlyNewBeneficiaries  ?? new(),
                MonthlyAidRequests       = snapshot.MonthlyAidRequests        ?? new(),
                MonthlyAidDisbursements  = snapshot.MonthlyAidDisbursements   ?? new(),
                MonthlyAidDisbursedAmount= snapshot.MonthlyAidDisbursedAmount ?? new(),
                MonthlyNewDonors         = snapshot.MonthlyNewDonors          ?? new(),
                MonthlyGrantsReceived    = snapshot.MonthlyGrantsReceived     ?? new(),
                GrantAgreementsByStatus  = snapshot.GrantAgreementsByStatus   ?? new(),
                MonthlyNewProjects       = snapshot.MonthlyNewProjects        ?? new(),
                ProjectsByStatus         = snapshot.ProjectsByStatus          ?? new(),
                MonthlyProjectBudget     = snapshot.MonthlyProjectBudget      ?? new(),
            };

            return View(model);
        }

        private static CharityDashboardVm CreateEmptyCharityDashboardVm()
        {
            return new CharityDashboardVm
            {
                BeneficiaryStatuses = new(),
                MonthlyDonations = new(),
                TopProjects = new(),
                RecentPayrollMonths = new(),
                StoreMovements = new()
            };
        }

        public IActionResult DashboardBlog()
        {
            return View();
        }

        public IActionResult DashboardCrypto()
        {
            return View();
        }

        public IActionResult Dashboardjob()
        {
            return View();
        }

        public IActionResult DashboardSaas()
        {
            return View();
        }

        public async Task<ActionResult> DashboardAccounts()
        {
            ViewBag.Title = "لوحة التحكم";
            ViewBag.pTitle = "لوحة التحكم";
            ViewBag.pageTitle = "الرئيسية";

            var items = await _itemRepository.GetAllAsync();
            var warehouses = await _warehouseRepository.GetAllAsync();
            var sales = await _salesInvoiceRepository.GetAllAsync();
            var purchases = await _purchaseInvoiceRepository.GetAllAsync();
            var holds = await _posHoldRepository.GetHeldAsync();
            var balances = await _itemWarehouseBalanceRepository.GetAllAsync();
            var paymentMethods = await _paymentMethodRepository.GetActiveAsync();

            var today = DateTime.Today;

            var model = new DashboardIndexVm
            {
                ItemsCount = items.Count,
                WarehousesCount = warehouses.Count,
                SalesTodayCount = sales.Count(x => x.InvoiceDateUtc.Date == today),
                PurchasesTodayCount = purchases.Count(x => x.InvoiceDateUtc.Date == today),
                HeldInvoicesCount = holds.Count,
                StockBalancesCount = balances.Count,
                ActivePaymentMethodsCount = paymentMethods.Count,
                LowStockCount = balances.Count(x =>
                    x.Item != null &&
                    x.Item.IsActive &&
                    x.Item.IsStockItem &&
                    x.Item.MinimumQuantity > 0 &&
                    x.AvailableQuantity <= x.Item.MinimumQuantity),

                LatestSales = sales
                    .OrderByDescending(x => x.InvoiceDateUtc)
                    .Take(5)
                    .Select(x => new DashboardInvoiceVm
                    {
                        Number = x.InvoiceNumber,
                        DateUtc = x.InvoiceDateUtc,
                        PartyName = x.CustomerName,
                        WarehouseName = x.Warehouse != null ? x.Warehouse.WarehouseNameAr : "-",
                        NetAmount = x.NetAmount
                    })
                    .ToList(),

                LatestPurchases = purchases
                    .OrderByDescending(x => x.InvoiceDateUtc)
                    .Take(5)
                    .Select(x => new DashboardInvoiceVm
                    {
                        Number = x.InvoiceNumber,
                        DateUtc = x.InvoiceDateUtc,
                        PartyName = x.SupplierName,
                        WarehouseName = x.Warehouse != null ? x.Warehouse.WarehouseNameAr : "-",
                        NetAmount = x.NetAmount
                    })
                    .ToList()
            };

            return View(model);
        }
    }
}
