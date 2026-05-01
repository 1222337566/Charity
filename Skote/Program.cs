using skote.Hubs;
using InfrastrfuctureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Reports;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.company;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.salesreturn;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;




//using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouse;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Domains.Jobs;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.NewFolder1;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Persistence.Repositories.Ef;
using InfrastructureManagmentDataAccess;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.Auditing;
using InfrastructureManagmentDataAccess.EntityFramework.EF;
using InfrastructureManagmentDataAccess.Repositories;
using InfrastructureManagmentDataAccess.Repositories.Charity;
using InfrastructureManagmentDataAccess.Repositories.Charity.Kafala;
using InfrastructureManagmentDataAccess.Repositories.Charity.StockAdvanced;
using InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers;
using InfrastructureManagmentDataAccess.Repositories.HR;
using InfrastructureManagmentDataAccess.Repositories.HR.Advanced;
using InfrastructureManagmentDataAccess.Repositories.Payroll;
using InfrastructureManagmentDataAccess.Repositories.Reports;
using InfrastructureManagmentServices.Authentication;
using InfrastructureManagmentServices.Charity.HumanitarianResearch;
using InfrastructureManagmentServices.Charity.Kafala;
using InfrastructureManagmentServices.Charity.ProjectProposals;
using InfrastructureManagmentServices.Charity.Funding;
using InfrastructureManagmentServices.Charity.Workflow;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastructureManagmentServices.CharityReports;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentServices.FileStorage;
using InfrastructureManagmentServices.Kanban;
using InfrastructureManagmentServices.Message;
using InfrastructureManagmentServices.Notification;
using InfrastructureManagmentServices.Optics;
using InfrastructureManagmentServices.PosHolds;
using InfrastructureManagmentServices.Profile;
using InfrastructureManagmentServices.Purchase;
using InfrastructureManagmentServices.Register;
using InfrastructureManagmentServices.Reporting;
using InfrastructureManagmentServices.Sales;
using InfrastructureManagmentServices.Salesreturn;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWeb.Seeding;
using InfrastructureManagmentWebFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SInfrastructureManagmentServices.CharityReports;
using Skote.Helpers;
using Skote.Seeding;
using Microsoft.AspNetCore.Http.Features;
var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<SignalRHubsOptions>(builder.Configuration.GetSection("SignalR"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// MVC + Razor + Swagger
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
// Add services to the container.
// Services
//authorization 
// بعد AddIdentity(...)
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = 20000;
    options.KeyLengthLimit = 4096;
    options.ValueLengthLimit = 1024 * 1024;
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});
builder.Services.AddAuthorization(options =>
{
    // حماية كل الصفحات افتراضيًا — إلا اللي عليها [AllowAnonymous]
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // سياسات جاهزة
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("CanViewLogs", p => p.RequireClaim("permission", "logs.read"));
});
builder.Services.AddCharityModuleAuthorization();
builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
{
    options.Conventions.Add(new CharityControllerAuthorizationConvention());
});
// خيارات الكوكيز: مسار صفحة الدخول والرفض
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.AccessDeniedPath = "/Account/Denied";
});
builder.Services.AddHttpClient<IRemoteNotifyClient, RemoteNotifyClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Realtime:BaseUrl"]!);
}).AddHttpMessageHandler<BearerFromRealtimeEndpointHandler>();
builder.Services.AddScoped<IAuthservice, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserActivityLogRepository, UserActivityLogRepository>();
builder.Services.AddScoped<ICurrentUserTokenFetcher, CurrentUserTokenFetcher>();
builder.Services.AddTransient<BearerFromRealtimeEndpointHandler>();
builder.Services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
builder.Services.AddScoped<IAccountingReportRepository, AccountingReportRepository>();
builder.Services.AddScoped<IProjectPlanningRepository, ProjectPlanningRepository>();
// أو لو تستخدم Generic فقط:
builder.Services.AddScoped<IBaseRepository<UserActivityLog>, BaseRepository<UserActivityLog>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserActivityAuditFilter>();
builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
{
    options.Filters.AddService<UserActivityAuditFilter>();
});
builder.Services.AddScoped<IPersonalInformationRepository, PersonalInformationRepository>();
builder.Services.AddScoped<IFileStorage, FileSystemFileStorage>();
builder.Services.AddScoped<IRegisterationService, RegisterationService>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<IAccountingIntegrationProfileRepository, AccountingIntegrationProfileRepository>();
builder.Services.AddScoped<IOperationalJournalService, OperationalJournalService>();
builder.Services.AddScoped<IProjectPhaseExpenseLinkRepository, ProjectPhaseExpenseLinkRepository>();
builder.Services.AddScoped<IProjectPhaseStoreIssueLinkRepository, ProjectPhaseStoreIssueLinkRepository>();
builder.Services.AddScoped<IProjectPhaseAccountingReportRepository, ProjectPhaseAccountingReportRepository>();
builder.Services.AddScoped<IOperationalJournalHookService, OperationalJournalHookService>();
builder.Services.AddScoped<IProjectPhaseActivityRepository, ProjectPhaseActivityRepository>();
builder.Services.AddScoped<IProjectPhaseTaskRepository, ProjectPhaseTaskRepository>();
builder.Services.AddScoped<IProjectTaskDailyUpdateRepository, ProjectTaskDailyUpdateRepository>();
builder.Services.AddScoped<IVolunteerSkillDefinitionRepository, VolunteerSkillDefinitionRepository>();
builder.Services.AddScoped<IVolunteerSkillRepository, VolunteerSkillRepository>();
builder.Services.AddScoped<IVolunteerAvailabilitySlotRepository, VolunteerAvailabilitySlotRepository>();
builder.Services.AddScoped<IVolunteerDirectoryRepository, VolunteerDirectoryRepository>();
builder.Services.AddScoped<IProjectTaskTrackingRepository, ProjectTaskTrackingRepository>();
builder.Services.AddScoped<IOperationalJournalService, OperationalJournalService>();
builder.Services.AddScoped<IOperationalJournalHookService, OperationalJournalHookService>();
builder.Services.AddScoped<IProjectAccountingProfileRepository, ProjectAccountingProfileRepository>();
builder.Services.AddScoped<IProjectExpenseLinkRepository, ProjectExpenseLinkRepository>();
builder.Services.AddScoped<IProjectAccountingReportRepository, ProjectAccountingReportRepository>();
builder.Services.AddScoped<IOpticalWorkOrderService, OpticalWorkOrderService>();
builder.Services.AddScoped<ISendService, SendService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddHttpClient<IRealtimeClient, RealtimeClient>(c =>
{
    c.BaseAddress = new Uri("http://localhost:32775/");
    c.BaseAddress = new Uri(builder.Configuration["Realtime:BaseUrl"]!);
    // عنوان مشروع realtime
}).AddHttpMessageHandler<BearerFromRealtimeEndpointHandler>();
builder.Services.AddScoped<IBaseRepository<BackgroundJob>, BaseRepository<BackgroundJob>>();
builder.Services.AddScoped<IBaseRepository<Balance>, BaseRepository<Balance>>();
builder.Services.AddScoped<IBaseRepository<Requests>, BaseRepository<Requests>>();
builder.Services.AddScoped<IBaseRepository<Error>, BaseRepository<Error>>();
builder.Services.AddScoped<IOpticalWorkOrderRepository, OpticalWorkOrderRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICharityOperationNotificationService, CharityOperationNotificationService>();
builder.Services.AddScoped<ICostCenterRepository, CostCenterRepository>();
builder.Services.AddScoped<IFiscalPeriodRepository, FiscalPeriodRepository>();
builder.Services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();
builder.Services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<ISalesInvoiceRepository, SalesInvoiceRepository>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICharityDashboardRepository, CharityDashboardRepository>();
builder.Services.AddScoped<ICharityReportsRepository, CharityReportsRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IItemWarehouseBalanceRepository, ItemWarehouseBalanceRepository>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<ICharityRfpReportRepository, CharityRfpReportRepository>();
builder.Services.AddScoped<ICharityWordExportService, CharityWordExportService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPayrollMonthRepository, PayrollMonthRepository>();
builder.Services.AddScoped<ISalaryItemDefinitionRepository, SalaryItemDefinitionRepository>();
builder.Services.AddScoped<IEmployeeSalaryStructureRepository, EmployeeSalaryStructureRepository>();
builder.Services.AddScoped<IPayrollEmployeeRepository, PayrollEmployeeRepository>();
builder.Services.AddScoped<IHrDepartmentRepository, HrDepartmentRepository>();
builder.Services.AddScoped<IHrJobTitleRepository, HrJobTitleRepository>();
builder.Services.AddScoped<IHrEmployeeRepository, HrEmployeeRepository>();
builder.Services.AddScoped<IHrShiftRepository, HrShiftRepository>();
builder.Services.AddScoped<IHrAttendanceRecordRepository, HrAttendanceRecordRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IDirectoryUserRepository, DirectoryUserRepository>();
builder.Services.AddScoped<IFunderRepository, FunderRepository>();
builder.Services.AddScoped<ICharityProjectRepository, CharityProjectRepository>();
builder.Services.AddScoped<IProjectBudgetLineRepository, ProjectBudgetLineRepository>();
builder.Services.AddScoped<IProjectActivityRepository, ProjectActivityRepository>();
builder.Services.AddScoped<IProjectBeneficiaryRepository, ProjectBeneficiaryRepository>();
builder.Services.AddScoped<IProjectGrantRepository, ProjectGrantRepository>();
builder.Services.AddScoped<IGrantAgreementRepository, GrantAgreementRepository>();
builder.Services.AddScoped<IGrantInstallmentRepository, GrantInstallmentRepository>();
builder.Services.AddScoped<IGrantConditionRepository, GrantConditionRepository>();
builder.Services.AddScoped<ICharityStoreReceiptRepository, CharityStoreReceiptRepository>();
builder.Services.AddScoped<ICharityStoreIssueRepository, CharityStoreIssueRepository>();
builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
builder.Services.AddScoped<ILdapDirectoryService, LdapDirectoryService>();
builder.Services.AddScoped<IAdImportService, AdImportService>();
builder.Services.AddScoped<IStockNeedRequestRepository, StockNeedRequestRepository>();
builder.Services.AddScoped<IStockReturnVoucherRepository, StockReturnVoucherRepository>();
builder.Services.AddScoped<IStockDisposalVoucherRepository, StockDisposalVoucherRepository>();
builder.Services.AddScoped<ICharityOperationsRepository, CharityOperationsRepository>();
builder.Services.AddScoped<IPosHoldRepository, PosHoldRepository>();
builder.Services.AddScoped<IPosHoldService, PosHoldService>();
builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
builder.Services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();
builder.Services.AddScoped<ISalesReturnService, SalesReturnService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ICharityReportExportService, CharityReportExportService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IBeneficiaryFamilyMemberRepository, BeneficiaryFamilyMemberRepository>();
builder.Services.AddScoped<IBeneficiaryDocumentRepository, BeneficiaryDocumentRepository>();
builder.Services.AddScoped<IBeneficiaryAssessmentRepository, BeneficiaryAssessmentRepository>();
builder.Services.AddScoped<IBeneficiaryCommitteeDecisionRepository, BeneficiaryCommitteeDecisionRepository>();
builder.Services.AddScoped<IBeneficiaryOldRecordRepository, BeneficiaryOldRecordRepository>();
builder.Services.AddScoped<IBeneficiaryAidRequestRepository, BeneficiaryAidRequestRepository>();
builder.Services.AddScoped<IBeneficiaryAidDisbursementRepository, BeneficiaryAidDisbursementRepository>();
builder.Services.AddScoped<IAidRequestFundingService, AidRequestFundingService>();
builder.Services.AddScoped<ICharityOperationalStatusService, CharityOperationalStatusService>();
builder.Services.AddScoped<IAidDisbursementFundingLineService, AidDisbursementFundingLineService>();
builder.Services.AddScoped<IKafalaSponsorRepository, KafalaSponsorRepository>();
builder.Services.AddScoped<IKafalaCaseRepository, KafalaCaseRepository>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IBoardMeetingRepository, BoardMeetingRepository>();
builder.Services.AddScoped<IUserActivityLogRepository, UserActivityLogRepository>();
builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Lookups.ICharityLookupRepository, InfrastructureManagmentDataAccess.Repositories.Charity.CharityLookupRepository>();
builder.Services.AddScoped<IBoardDecisionRepository, BoardDecisionRepository>();
builder.Services.AddScoped<IBoardDecisionFollowUpRepository, BoardDecisionFollowUpRepository>();
builder.Services.AddScoped<IVolunteerProjectAssignmentRepository, VolunteerProjectAssignmentRepository>();
builder.Services.AddScoped<IBoardDecisionAttachmentRepository, BoardDecisionAttachmentRepository>();
builder.Services.AddScoped<IAidCycleReminderService, AidCycleReminderService>();
builder.Services.AddScoped<IProjectProposalRepository, ProjectProposalRepository>();
builder.Services.AddScoped<IProjectProposalConversionService, ProjectProposalConversionService>();
builder.Services.AddScoped<IVolunteerHourLogRepository, VolunteerHourLogRepository>();
builder.Services.AddScoped<IKafalaPaymentRepository, KafalaPaymentRepository>();
builder.Services.AddScoped<IProjectPhaseRepository, ProjectPhaseRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDonationAllocationRepository, DonationAllocationRepository>();
builder.Services.AddScoped<IProjectTrackingLogRepository, ProjectTrackingLogRepository>();
builder.Services.AddScoped<ICharityOperationNotificationService, CharityOperationNotificationService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<ITaskBoardRepository, TaskBoardRepository>();
builder.Services.AddScoped<IWorkflowCompletionHandler, WorkflowCompletionHandler>();
builder.Services.AddScoped<IKanbanService, SimpleKanbanService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromHours(8); o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });
builder.Services.AddScoped<IDonationInKindItemRepository, DonationInKindItemRepository>();
builder.Services.AddScoped<IBeneficiaryHumanitarianResearchRepository, BeneficiaryHumanitarianResearchRepository>();
builder.Services.AddScoped<InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries.IBeneficiaryDocumentRepository, InfrastructureManagmentDataAccess.Repositories.Charity.BeneficiaryDocumentRepository>();
//builder.Services.AddScoped<InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries.IBeneficiaryHumanitarianResearchRepository, InfrastructureManagmentDataAccess.Repositories.Charity.BeneficiaryHumanitarianResearchRepository>();
builder.Services.AddScoped<IHumanitarianResearchWorkflowService, HumanitarianResearchWorkflowService>();
builder.Services.AddScoped<IKafalaAidCycleBridgeService, KafalaAidCycleBridgeService>();
builder.Services.AddScoped<IAvailableAidDisbursementService, AvailableAidDisbursementService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ICustomerOldRecordRepository, CustomerOldRecordRepository>();
builder.Services.AddScoped<IHrEmployeeMovementRepository, HrEmployeeMovementRepository>();
builder.Services.AddScoped<IHrSanctionRecordRepository, HrSanctionRecordRepository>();
builder.Services.AddScoped<IProjectPhaseMilestoneRepository, ProjectPhaseMilestoneRepository>();
builder.Services.AddScoped<IHrOutRequestRepository, HrOutRequestRepository>();
builder.Services.AddScoped<IHrPerformanceEvaluationRepository, HrPerformanceEvaluationRepository>();
builder.Services.AddScoped<ICustomerReceiptService, CustomerReceiptService>();
builder.Services.AddScoped<InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles.IAidCycleRepository, InfrastructureManagmentDataAccess.Repositories.Charity.AidCycleRepository>();
builder.Services.AddScoped<InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles.IAidCycleBeneficiaryRepository, InfrastructureManagmentDataAccess.Repositories.Charity.AidCycleBeneficiaryRepository>();
builder.Services.AddScoped<ICustomerReceiptRepository, CustomerReceiptRepository>();
builder.Services.AddScoped<InfrastructureManagmentServices.Charity.AidCycles.IAidCycleGenerationService, InfrastructureManagmentServices.Charity.AidCycles.AidCycleGenerationService>();
builder.Services.AddScoped<InfrastructureManagmentServices.Charity.AidCycles.IAidCycleDisbursementService, InfrastructureManagmentServices.Charity.AidCycles.AidCycleDisbursementService>();
builder.Services.AddScoped<ICustomerAccountService, CustomerAccountService>();
builder.Services.AddScoped<ICustomerAccountTransactionRepository, CustomerAccountTransactionRepository>();
builder.Services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
builder.Services.AddScoped<IDataCollector<ReceiveModel>, DataCollector<ReceiveModel>>();
builder.Services.AddScoped<IDataCollector<HTSSendSMSModel>, DataCollector<HTSSendSMSModel>>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddDbContext<AppDbContext>(
               options => options.UseSqlServer(builder.Configuration.GetConnectionString(@"BillingWebAPIDbconnection")
));
// IDbContextFactory — يتيح إنشاء DbContext instances مستقلة للـ parallel queries
builder.Services.AddDbContextFactory<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString(@"BillingWebAPIDbconnection")),
    lifetime: ServiceLifetime.Scoped
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseStaticFiles();        // لعرض الصور من wwwroot
app.UseRouting();
app.UseAuthentication();     // لازم قبل UseAuthorization (كان متعلّق في كودك):contentReference[oaicite:3]{index=3}
app.Use(async (context, next) =>
{
    var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                 ?? context.User?.FindFirst("sub")?.Value;
    var userName = context.User?.Identity?.Name
                   ?? context.User?.FindFirst("name")?.Value
                   ?? context.User?.FindFirst("preferred_username")?.Value;

    AuditUserContext.Set(userId, userName);
    try
    {
        await next();
    }
    finally
    {
        AuditUserContext.Clear();
    }
});
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CharityDashboard}/{action=Index}/{id?}");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    //await db.Database.MigrateAsync();
    await IdentitySeeder.SeedAsync(app.Services);
    await AccountSeeder.SeedAsync(db);
    await AccountingPostingProfileSeeder.SeedAsync(db);
    await PosBasicSeeder.SeedUnitsAndGroupsAsync(db);
    await GreenVisionSeeder.SeedAsync(db);
    await PosBasicSeeder.SeedBeneficiaryCategoriesAsync(db);
}

await CharityIdentitySeeder.SeedAsync(app.Services);
app.MapHub<KanbanHub>("/hubs/kanban");
app.Run();
