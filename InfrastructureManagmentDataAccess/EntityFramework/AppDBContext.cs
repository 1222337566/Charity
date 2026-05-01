//using BillingCore.Domains.Messages;

using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Company;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.PosHolds;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Domains.Progiling;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.supplies;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentCore.Domains.Billing;
using InfrastructureManagmentCore.Domains.Calendar;
using InfrastructureManagmentCore.Domains.Chat;
using InfrastructureManagmentCore.Domains.Complains;
using InfrastructureManagmentCore.Domains.Connection;
using InfrastructureManagmentCore.Domains.Documents;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Domains.Jobs;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.NewFolder1;
using InfrastructureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Domains.Products;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentCore.Domains.Projects;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentCore.Domains.supplies;
using InfrastructureManagmentDataAccess.EntityFramework.EF;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Kafala;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Workflow;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Configurations.Charity;
using InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced;
using InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Rfp;
using MaintainanceSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Beneficiaries;
//using Microsoft.AspNet.Identity.EntityFramework;

namespace InfrastructureManagmentDataAccess.EntityFramework
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //options = AppDbContext.GetOptions();
        }
        //public static DbContextOptions GetOptions()
        //{
        //    return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), "Data Source=DPP102;Initial Catalog=Infra;User Id=billingWebapi;Password=ax,%E`bt&N/F^nv#)f6k+; TrustServerCertificate=True;").Options;
        //}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new StockTransactionConfiguration());
            builder.ApplyConfiguration(new ItemWarehouseBalanceConfiguration());
            builder.ApplyConfiguration(new AccountConfiguration());
            builder.ApplyConfiguration(new ItemGroupConfiguration());
            builder.ApplyConfiguration(new BeneficiaryContactLogConfiguration());
            builder.ApplyConfiguration(new CharityStoreReceiptConfiguration());
            ////////////////////////
            builder.ApplyConfiguration(new GovernorateConfiguration());
            builder.ApplyConfiguration(new BeneficiaryCategoryConfiguration());
            builder.ApplyConfiguration(new BeneficiaryCategoryAssignmentConfiguration());
            builder.ApplyConfiguration(new CityConfiguration());
            builder.ApplyConfiguration(new AreaConfiguration());
            builder.ApplyConfiguration(new GenderLookupConfiguration());
            builder.ApplyConfiguration(new MaritalStatusLookupConfiguration());
            builder.ApplyConfiguration(new BeneficiaryStatusLookupConfiguration());
            builder.ApplyConfiguration(new AidTypeLookupConfiguration());
            builder.ApplyConfiguration(new DonorConfiguration());
            builder.ApplyConfiguration(new DonationConfiguration());
            builder.ApplyConfiguration(new HrDepartmentConfiguration());
            builder.ApplyConfiguration(new HrJobTitleConfiguration());
            builder.ApplyConfiguration(new HrEmployeeConfiguration());
            builder.ApplyConfiguration(new HrShiftConfiguration());
            builder.ApplyConfiguration(new HrAttendanceRecordConfiguration());
            builder.ApplyConfiguration(new BeneficiaryConfiguration());
            builder.ApplyConfiguration(new BeneficiaryFamilyMemberConfiguration());
            builder.ApplyConfiguration(new BeneficiaryDocumentConfiguration());
            builder.ApplyConfiguration(new BeneficiaryAssessmentConfiguration());
            builder.ApplyConfiguration(new BeneficiaryCommitteeDecisionConfiguration());
            builder.ApplyConfiguration(new BeneficiaryOldRecordConfiguration());
            builder.ApplyConfiguration(new BeneficiaryAidRequestConfiguration());
            builder.ApplyConfiguration(new BeneficiaryAidRequestLineConfiguration());
            builder.ApplyConfiguration(new DonationInKindItemConfiguration());
            builder.ApplyConfiguration(new DonationAllocationConfiguration());
            builder.ApplyConfiguration(new BeneficiaryAidDisbursementConfiguration());
            builder.ApplyConfiguration(new BeneficiaryAidDisbursementFundingLineConfiguration());
            builder.ApplyConfiguration(new FunderConfiguration());
            builder.ApplyConfiguration(new GrantAgreementConfiguration());
            builder.ApplyConfiguration(new CharityStoreReceiptConfiguration());
            builder.ApplyConfiguration(new CharityStoreReceiptLineConfiguration());
            builder.ApplyConfiguration(new CharityStoreIssueConfiguration());
            builder.ApplyConfiguration(new CharityStoreIssueLineConfiguration());
            builder.ApplyConfiguration(new GrantInstallmentConfiguration());
            builder.ApplyConfiguration(new CharityProjectConfiguration());
            builder.ApplyConfiguration(new ProjectBudgetLineConfiguration());
            builder.ApplyConfiguration(new ProjectActivityConfiguration());
            builder.ApplyConfiguration(new ProjectBeneficiaryConfiguration());
            builder.ApplyConfiguration(new ProjectGrantConfiguration());
            builder.ApplyConfiguration(new GrantConditionConfiguration());
            builder.ApplyConfiguration(new AccountingIntegrationProfileConfiguration());
            builder.ApplyConfiguration(new AccountingIntegrationSourceDefinitionConfiguration());
            builder.ApplyConfiguration(new AccountingPostingProfileConfiguration());
            builder.ApplyConfiguration(new HrEmployeeMovementConfiguration());
            builder.ApplyConfiguration(new HrSanctionRecordConfiguration());
            builder.ApplyConfiguration(new HrOutRequestConfiguration());
            builder.ApplyConfiguration(new HrLeaveTypeConfiguration());
            builder.ApplyConfiguration(new HrLeaveRequestConfiguration());
            builder.ApplyConfiguration(new HrLeaveBalanceConfiguration());
            builder.ApplyConfiguration(new HrPerformanceEvaluationConfiguration());

            ///
            builder.ApplyConfiguration(new PosHoldConfiguration());
            builder.ApplyConfiguration(new CustomerReceiptConfiguration());
            builder.ApplyConfiguration(new PosHoldLineConfiguration());
            builder.ApplyConfiguration(new PrescriptionConfiguration());
            builder.ApplyConfiguration(new ProjectConfiguration());
            builder.ApplyConfiguration(new ProjectPhaseConfiguration());
            builder.ApplyConfiguration(new ProjectPhaseMilestoneConfiguration());
            builder.ApplyConfiguration(new ProjectTrackingLogConfiguration());
            builder.ApplyConfiguration(new ExpenseCategoryConfiguration());
            builder.ApplyConfiguration(new CustomerAccountTransactionConfiguration());
            builder.ApplyConfiguration(new SalesReturnInvoiceConfiguration());
            builder.ApplyConfiguration(new PayrollMonthConfiguration());
            builder.ApplyConfiguration(new AidCycleConfiguration());

            builder.ApplyConfiguration(new AidCycleBeneficiaryConfiguration());
            builder.ApplyConfiguration(new SalaryItemDefinitionConfiguration());
            builder.ApplyConfiguration(new EmployeeSalaryStructureConfiguration());
            builder.ApplyConfiguration(new PayrollEmployeeConfiguration());
            builder.ApplyConfiguration(new PayrollEmployeeItemConfiguration());
            builder.ApplyConfiguration(new PayrollPaymentConfiguration());
            builder.ApplyConfiguration(new CostCenterConfiguration());
            builder.ApplyConfiguration(new FiscalPeriodConfiguration());
            builder.ApplyConfiguration(new JournalEntryConfiguration());
            builder.ApplyConfiguration(new JournalEntryLineConfiguration());
            builder.ApplyConfiguration(new SalesReturnLineConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new OpticalWorkOrderConfiguration());
            builder.ApplyConfiguration(new VolunteerConfiguration());
            builder.ApplyConfiguration(new VolunteerProjectAssignmentConfiguration());
            builder.ApplyConfiguration(new VolunteerHourLogConfiguration());
            builder.ApplyConfiguration(new SupplierConfiguration());
            builder.ApplyConfiguration(new SupplierPaymentConfiguration());
            builder.ApplyConfiguration(new KafalaSponsorConfiguration());
            builder.ApplyConfiguration(new BoardMeetingConfiguration());
            builder.ApplyConfiguration(new BoardMeetingAttendeeConfiguration());
            builder.ApplyConfiguration(new BoardMeetingMinuteConfiguration());
            builder.ApplyConfiguration(new BoardDecisionConfiguration());
            builder.ApplyConfiguration(new BoardDecisionFollowUpConfiguration());
            builder.ApplyConfiguration(new BoardMeetingAttachmentConfiguration());
            builder.ApplyConfiguration(new KafalaCaseConfiguration());
            builder.ApplyConfiguration(new ProjectTeamMemberConfiguration());
            builder.ApplyConfiguration(new ProjectTeamMemberAttachmentConfiguration());

            builder.ApplyConfiguration(new KafalaPaymentConfiguration());
            builder.ApplyConfiguration(new WorkflowStepConfiguration());
            builder.ApplyConfiguration(new ProjectPhaseExpenseLinkConfiguration());
            builder.ApplyConfiguration(new ProjectPhaseStoreIssueLinkConfiguration());
            builder.ApplyConfiguration(new HrEmployeeContractConfiguration());
            builder.ApplyConfiguration(new HrEmployeeFundingAssignmentConfiguration());
            builder.ApplyConfiguration(new HrEmployeeTaskAssignmentConfiguration());
            builder.ApplyConfiguration(new HrEmployeeBonusConfiguration());
            builder.ApplyConfiguration(new ProjectProposalConfiguration());
            builder.ApplyConfiguration(new ProjectActivityBeneficiaryConfiguration());
            builder.ApplyConfiguration(new ProjectActivityBeneficiaryAttachmentConfiguration());

            builder.ApplyConfiguration(new ProjectProposalPastExperienceConfiguration());
            builder.ApplyConfiguration(new ProjectProposalTargetGroupConfiguration());
            builder.ApplyConfiguration(new ProjectProposalObjectiveConfiguration());
            builder.ApplyConfiguration(new ProjectProposalActivityConfiguration());
            builder.ApplyConfiguration(new ProjectProposalWorkPlanConfiguration());
            builder.ApplyConfiguration(new ProjectProposalMonitoringIndicatorConfiguration());
            builder.ApplyConfiguration(new ProjectProposalTeamMemberConfiguration());
            builder.ApplyConfiguration(new ProjectProposalAttachmentConfiguration());
            builder.ApplyConfiguration(new ProjectProposalActivityConfiguration());
            builder.ApplyConfiguration(new ProjectProposalPhaseConfiguration());
            builder.ApplyConfiguration(new ProjectProposalPhaseActivityConfiguration());
            builder.ApplyConfiguration(new ProjectGoalConfiguration());
            builder.ApplyConfiguration(new ProjectSubGoalConfiguration());
            builder.ApplyConfiguration(new ProjectSubGoalActivityConfiguration());
            builder.ApplyConfiguration(new ActivityPhaseAssignmentConfiguration());
            builder.ApplyConfiguration(new ProjectPhaseTaskConfiguration());
            builder.ApplyConfiguration(new ProjectTaskDailyUpdateConfiguration());
            builder.ApplyConfiguration(new CustomerOldRecordConfiguration());
            builder.ApplyConfiguration(new CompanyProfileConfiguration());
            builder.ApplyConfiguration(new SalesInvoicePaymentConfiguration());
            builder.ApplyConfiguration(new ProjectAccountingProfileConfiguration());
            builder.ApplyConfiguration(new StockNeedRequestConfiguration());
            builder.ApplyConfiguration(new BoardDecisionAttachmentConfiguration());
            builder.ApplyConfiguration(new StockNeedRequestLineConfiguration());
            builder.ApplyConfiguration(new StockReturnVoucherConfiguration());
            builder.ApplyConfiguration(new StockReturnVoucherLineConfiguration());
            builder.ApplyConfiguration(new VolunteerSkillDefinitionConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchFamilyMemberConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchIncomeItemConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchExpenseItemConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchDebtConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchHouseAssetConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchReviewConfiguration());
            builder.ApplyConfiguration(new BeneficiaryHumanitarianResearchCommitteeEvaluationConfiguration());
            builder.ApplyConfiguration(new VolunteerSkillConfiguration());
            builder.ApplyConfiguration(new VolunteerAvailabilitySlotConfiguration());
            builder.ApplyConfiguration(new StockDisposalVoucherConfiguration());
            builder.ApplyConfiguration(new StockDisposalVoucherLineConfiguration());
            builder.ApplyConfiguration(new ProjectExpenseLinkConfiguration());
            builder.ApplyConfiguration(new SalesInvoiceConfiguration());
            builder.ApplyConfiguration(new SalesInvoiceLineConfiguration());
            builder.ApplyConfiguration(new PaymentMethodConfiguration());
            builder.ApplyConfiguration(new ExpenseConfiguration());
            builder.ApplyConfiguration(new UnitConfiguration());
            builder.ApplyConfiguration(new ExpenseCategoryConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new PurchaseInvoiceConfiguration());
            builder.ApplyConfiguration(new PurchaseInvoiceLineConfiguration());
            builder.ApplyConfiguration(new WarehouseConfiguration());
            builder.Entity<BoardUser>().HasKey(x => new { x.BoardId, x.UserId });
            builder.Entity<DirectoryDevice>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.DirectoryUserId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.Property(x => x.HostName).HasMaxLength(200).IsRequired();
                b.Property(x => x.Source).HasMaxLength(50);
            });
            builder.Entity<DirectoryUser>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.AdObjectId).IsUnique();
                b.HasIndex(x => x.Upn);
                b.Property(x => x.AdObjectId).HasMaxLength(64).IsRequired();
                b.Property(x => x.Upn).HasMaxLength(256).IsRequired();
                b.Property(x => x.Email).HasMaxLength(256);
                b.Property(x => x.DisplayName).HasMaxLength(256);
                b.Property(x => x.Department).HasMaxLength(128);
                b.Property(x => x.DistinguishedName).HasMaxLength(512).IsRequired();
                b.Property(x => x.UserWorkstations).HasMaxLength(2048); // 
            });
            builder.Entity<TaskBoard>()
                .HasMany(x => x.Members)
                .WithOne(x => x.Board!)
                .HasForeignKey(x => x.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BoardUser>().HasIndex(x => new { x.UserId, x.BoardId });
            builder.Entity<BoardUser>().HasKey(x => new { x.BoardId, x.UserId });

            builder.Entity<KanbanTask>()
                .HasIndex(x => new { x.BoardId, x.Status, x.OrderIndex });
            builder.Entity<Notification2>()
      .HasIndex(x => new { x.CreatedByUserId, x.IsRead, x.CreatedAtUtc });
            builder.Entity<TaskAudit>()
                .HasIndex(x => new { x.TaskId, x.AtUtc });
            // ===== Notifications =====
            builder.Entity<Notification2>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200);
                e.Property(x => x.Level).HasMaxLength(20);

                // CreatedByUserId -> AspNetUsers(Id)
                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.CreatedByUserId)
                  .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<NotificationDelivery>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Notification)
                  .WithMany(n => n.Deliveries)
                  .HasForeignKey(x => x.NotificationId)
                  .OnDelete(DeleteBehavior.Cascade);

                // UserId -> AspNetUsers(Id)
                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.UserId, x.IsRead });
            });
            // Add column OrderIndex default 0 and index

            // ===== Chat =====
            builder.Entity<ChatRoom>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
            });

            builder.Entity<ChatMessage2>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Room)
                  .WithMany()
                  .HasForeignKey(x => x.RoomId)
                  .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.FromUserId)
                  .OnDelete(DeleteBehavior.NoAction);

                // لو بتستخدم رسائل خاصة
                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.ToUserId)
                  .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => new { x.RoomId, x.SentAtUtc });
            });

            // ===== Calendar =====
            builder.Entity<CalendarEvent>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200);
                e.HasIndex(x => new { x.StartUtc, x.EndUtc });

                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.CreatedByUserId)
                  .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<CalendarAttendee>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Event)
                  .WithMany(ev => ev.Attendees)
                  .HasForeignKey(x => x.EventId)
                  .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<ApplicationUser>().WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.EventId, x.UserId }).IsUnique();
            });

            builder.Entity<EventReminder>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Event)
                  .WithMany(ev => ev.Reminders)
                  .HasForeignKey(x => x.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<ApplicationUser>()
                .ToTable("Users")
                .Ignore(e => e.TwoFactorEnabled)
                .Ignore(e => e.PhoneNumber)
                .Ignore(e => e.EmailConfirmed)
                .Ignore(e => e.PhoneNumberConfirmed)
                .Ignore(e => e.Email);

            builder.Entity<Receiver>()
                .HasIndex(p => p.telephonenum)
                .IsUnique();
            builder.Entity<ChatMessage>()
     .HasOne(m => m.sender)            // Navigation في ChatMessage إلى Employee المرسِل
     .WithMany(e => e.SMessages)       // مجموعة في Employee لرسائل مرسَلة
     .HasForeignKey(m => m.senderid)   // FK في ChatMessage
     .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.receiver)          // Navigation في ChatMessage إلى Employee المستقبِل
                .WithMany(e => e.RMessages)       // مجموعة في Employee لرسائل مستقبَلة
                .HasForeignKey(m => m.receiver_id) // FK في ChatMessage
                .OnDelete(DeleteBehavior.Restrict);


            builder.Ignore<CustomAttributeData>();

            // علاقات ChatMessage مع PersonalInformation
            builder.Entity<ChatMessage>()
                .HasOne(m => m.Spersonal)              // Navigation في ChatMessage للمرسِل (PersonalInformation)
                .WithMany(p => p.SMessages)             // مجموعة رسائل مُرسَلة في PersonalInformation
                .HasForeignKey(m => m.SPersonalid)     // FK في ChatMessage
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Rpersonal)            // Navigation في ChatMessage للمستقبِل (PersonalInformation)
                .WithMany(p => p.RMessages)             // مجموعة رسائل مُستقبَلة في PersonalInformation
                .HasForeignKey(m => m.RPersonalid)   // FK في ChatMessage
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NotificationDelivery>()
    .HasIndex(x => new { x.UserId, x.CreatedAtUtc });

            builder.Entity<Notification2>()
                .HasIndex(x => x.Kind);
            builder.Entity<Notification2>()
                .HasIndex(x => x.Level);
            builder.Entity<PersonalInformation>(e =>
            {
                e.HasKey(p => p.person_id);
                e.HasOne(p => p.User)
                 .WithOne() // أو WithOne(u => u.PersonalInformation) لو عندك Nav في ApplicationUser
                 .HasForeignKey<PersonalInformation>(p => p.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<UserActivityLog>(e =>
            {
                e.ToTable("UserActivityLogs");
                e.HasKey(x => x.Id);
                e.Property(x => x.Action).HasMaxLength(32);
                e.Property(x => x.IpAddress).HasMaxLength(64);
                e.Property(x => x.UserAgent).HasMaxLength(512);
            });

            builder.Entity<TaskBoard>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            });

            builder.Entity<TaskItem>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).HasMaxLength(256).IsRequired();
                b.Property(x => x.Status).HasMaxLength(32).HasDefaultValue("ToDo");
                b.Property(x => x.Priority).HasMaxLength(32).HasDefaultValue("Medium");
                b.HasOne(x => x.Board).WithMany(bd => bd.Tasks).HasForeignKey(x => x.BoardId);
                b.HasIndex(x => new { x.BoardId, x.Status });
            });
        }
        //public DbSet<HTSGroup> Groups { get; set; }
        public DbSet<DirectoryUser> DirectoryUsers => Set<DirectoryUser>();
        public DbSet<DirectoryDevice> DirectoryDevices { get; set; }
        public DbSet<ItemGroup> ItemGroups => Set<ItemGroup>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<CharityStoreReceipt> charityStoreReceipts => Set<CharityStoreReceipt>();
        public DbSet<PosHold> PosHolds => Set<PosHold>();
        public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
        public DbSet<PosHoldLine> PosHoldLines => Set<PosHoldLine>();
        public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<BeneficiaryCategory> BeneficiaryCategories { get; set; } = null!;
        public DbSet<BeneficiaryCategoryAssignment> BeneficiaryCategoryAssignments { get; set; } = null!;
        public DbSet<Volunteer> CharityVolunteers => Set<Volunteer>();
        public DbSet<VolunteerProjectAssignment> CharityVolunteerProjectAssignments => Set<VolunteerProjectAssignment>();
        public DbSet<VolunteerHourLog> CharityVolunteerHourLogs => Set<VolunteerHourLog>();
        public DbSet<HrDepartment> HrDepartments { get; set; }
        public DbSet<HrJobTitle> HrJobTitles { get; set; }
        public DbSet<ProjectProposalPhase> projectProposalPhases { get; set; }
        public DbSet<ProjectProposalPhaseActivity> projectProposalPhaseActivities { get; set; }
        public DbSet<BeneficiaryContactLog> beneficiaryContactLogs { get; set; }
        public DbSet<HrEmployee> HrEmployees { get; set; }
        public DbSet<HrShift> HrShifts { get; set; }
        public DbSet<AidCycle> CharityAidCycles { get; set; }
        public DbSet<VolunteerSkillDefinition> VolunteerSkillDefinitions => Set<VolunteerSkillDefinition>();
        public DbSet<VolunteerSkill> VolunteerSkills => Set<VolunteerSkill>();
        public DbSet<VolunteerAvailabilitySlot> VolunteerAvailabilitySlots => Set<VolunteerAvailabilitySlot>();
        public DbSet<AidCycleBeneficiary> CharityAidCycleBeneficiaries { get; set; }
        public DbSet<ProjectPhase> CharityProjectPhases { get; set; }
        public DbSet<ProjectPhaseExpenseLink> ProjectPhaseExpenseLinks => Set<ProjectPhaseExpenseLink>();
        public DbSet<ProjectPhaseStoreIssueLink> ProjectPhaseStoreIssueLinks => Set<ProjectPhaseStoreIssueLink>();
        public DbSet<ProjectPhaseMilestone> CharityProjectPhaseMilestones { get; set; }
        public DbSet<ProjectTrackingLog> CharityProjectTrackingLogs { get; set; }
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<ProjectActivityBeneficiary> CharityProjectActivityBeneficiaries => Set<ProjectActivityBeneficiary>();
        public DbSet<ProjectActivityBeneficiaryAttachment> CharityProjectActivityBeneficiaryAttachments => Set<ProjectActivityBeneficiaryAttachment>();

        public DbSet<ProjectAccountingProfile> ProjectAccountingProfiles { get; set; }
        public DbSet<ProjectExpenseLink> ProjectExpenseLinks { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
        public DbSet<CostCenter> CostCenters => Set<CostCenter>();
        public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
        public DbSet<AccountingIntegrationProfile> AccountingIntegrationProfiles => Set<AccountingIntegrationProfile>();
        public DbSet<AccountingIntegrationSourceDefinition> AccountingIntegrationSourceDefinitions => Set<AccountingIntegrationSourceDefinition>();
        public DbSet<ProjectPhaseActivity> CharityProjectPhaseActivities { get; set; }
        public DbSet<ProjectGoal> CharityProjectGoals => Set<ProjectGoal>();
        public DbSet<ProjectSubGoal> CharityProjectSubGoals => Set<ProjectSubGoal>();
        public DbSet<ProjectSubGoalActivity> CharityProjectSubGoalActivities => Set<ProjectSubGoalActivity>();
        public DbSet<ActivityPhaseAssignment> CharityActivityPhaseAssignments => Set<ActivityPhaseAssignment>();
        public DbSet<ProjectPhaseTask> CharityProjectPhaseTasks { get; set; }
        public DbSet<ProjectTaskDailyUpdate> CharityProjectTaskDailyUpdates { get; set; }
        public DbSet<HrAttendanceRecord> HrAttendanceRecords { get; set; }
        public DbSet<CharityStoreReceipt> CharityStoreReceipts => Set<CharityStoreReceipt>();
        public DbSet<CharityStoreReceiptLine> CharityStoreReceiptLines => Set<CharityStoreReceiptLine>();
        public DbSet<CharityStoreIssue> CharityStoreIssues => Set<CharityStoreIssue>();
        public DbSet<CharityStoreIssueLine> CharityStoreIssueLines => Set<CharityStoreIssueLine>();
        public DbSet<DonationInKindItem> CharityDonationInKindItems { get; set; }
        public DbSet<DonationAllocation> CharityDonationAllocations { get; set; }
        public DbSet<BeneficiaryHumanitarianResearch> CharityBeneficiaryHumanitarianResearchs { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchFamilyMember> CharityBeneficiaryHumanitarianResearchFamilyMembers { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchIncomeItem> CharityBeneficiaryHumanitarianResearchIncomeItems { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchExpenseItem> CharityBeneficiaryHumanitarianResearchExpenseItems { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchDebt> CharityBeneficiaryHumanitarianResearchDebts { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchHouseAsset> CharityBeneficiaryHumanitarianResearchHouseAssets { get; set; }
        public DbSet<BoardMeeting> CharityBoardMeetings { get; set; }
        public DbSet<BoardMeetingAttendee> CharityBoardMeetingAttendees { get; set; }
        public DbSet<BoardMeetingMinute> CharityBoardMeetingMinutes { get; set; }
        public DbSet<BoardDecision> CharityBoardDecisions { get; set; }
        public DbSet<BoardDecisionFollowUp> CharityBoardDecisionFollowUps { get; set; }
        public DbSet<BoardMeetingAttachment> CharityBoardMeetingAttachments { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchReview> CharityBeneficiaryHumanitarianResearchReviews { get; set; }
        public DbSet<BeneficiaryHumanitarianResearchCommitteeEvaluation> CharityBeneficiaryHumanitarianResearchCommitteeEvaluations { get; set; }
        public DbSet<Expensex> Expenses => Set<Expensex>();

        public DbSet<Funder> CharityFunders => Set<Funder>();
        public DbSet<GrantAgreement> CharityGrantAgreements => Set<GrantAgreement>();
        public DbSet<KafalaSponsor> KafalaSponsors => Set<KafalaSponsor>();
        public DbSet<KafalaCase> KafalaCases => Set<KafalaCase>();
        public DbSet<KafalaPayment> KafalaPayments => Set<KafalaPayment>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<GrantInstallment> CharityGrantInstallments => Set<GrantInstallment>();
        public DbSet<GrantCondition> CharityGrantConditions => Set<GrantCondition>();
        public DbSet<CustomerOldRecord> CustomerOldRecords => Set<CustomerOldRecord>();
        public DbSet<CustomerAccountTransaction> CustomerAccountTransactions => Set<CustomerAccountTransaction>();
        public DbSet<SalesReturnInvoice> SalesReturnInvoices => Set<SalesReturnInvoice>();
        public DbSet<OpticalWorkOrder> OpticalWorkOrders => Set<OpticalWorkOrder>();
        public DbSet<SalesReturnLine> SalesReturnLines => Set<SalesReturnLine>();
        public DbSet<SalesInvoicePayment> SalesInvoicePayments => Set<SalesInvoicePayment>();
        public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
        public DbSet<PurchaseInvoiceLine> PurchaseInvoiceLines => Set<PurchaseInvoiceLine>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        
        public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
        public DbSet<ItemWarehouseBalance> ItemWarehouseBalances => Set<ItemWarehouseBalance>();
        public DbSet<Item> Items => Set<Item>();
        /// <summary>
        public DbSet<Governorate> Governorates => Set<Governorate>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Area> Areas => Set<Area>();
        public DbSet<PayrollMonth> PayrollMonths => Set<PayrollMonth>();
        public DbSet<SalaryItemDefinition> SalaryItemDefinitions => Set<SalaryItemDefinition>();
        public DbSet<EmployeeSalaryStructure> EmployeeSalaryStructures => Set<EmployeeSalaryStructure>();
        public DbSet<PayrollEmployee> PayrollEmployees => Set<PayrollEmployee>();
        public DbSet<PayrollEmployeeItem> PayrollEmployeeItems => Set<PayrollEmployeeItem>();
        public DbSet<PayrollPayment> PayrollPayments => Set<PayrollPayment>();
        public DbSet<GenderLookup> GenderLookups => Set<GenderLookup>();
        public DbSet<MaritalStatusLookup> MaritalStatusLookups => Set<MaritalStatusLookup>();
        public DbSet<BeneficiaryStatusLookup> BeneficiaryStatusLookups => Set<BeneficiaryStatusLookup>();
        public DbSet<AidTypeLookup> AidTypeLookups => Set<AidTypeLookup>();
        public DbSet<StockNeedRequest> StockNeedRequests => Set<StockNeedRequest>();
        public DbSet<ProjectProposal> ProjectProposals => Set<ProjectProposal>();

        public DbSet<ProjectProposalPastExperience> ProjectProposalPastExperiences => Set<ProjectProposalPastExperience>();
        public DbSet<ProjectProposalTargetGroup> ProjectProposalTargetGroups => Set<ProjectProposalTargetGroup>();
        public DbSet<ProjectProposalObjective> ProjectProposalObjectives => Set<ProjectProposalObjective>();
        public DbSet<ProjectProposalActivity> ProjectProposalActivities => Set<ProjectProposalActivity>();
        public DbSet<ProjectProposalWorkPlan> ProjectProposalWorkPlans => Set<ProjectProposalWorkPlan>();
        public DbSet<ProjectProposalMonitoringIndicator> ProjectProposalMonitoringIndicators => Set<ProjectProposalMonitoringIndicator>();
        public DbSet<ProjectProposalTeamMember> ProjectProposalTeamMembers => Set<ProjectProposalTeamMember>();
        public DbSet<ProjectProposalAttachment> ProjectProposalAttachments => Set<ProjectProposalAttachment>();
        public DbSet<StockNeedRequestLine> StockNeedRequestLines => Set<StockNeedRequestLine>();
        public DbSet<StockReturnVoucher> StockReturnVouchers => Set<StockReturnVoucher>();
        public DbSet<StockReturnVoucherLine> StockReturnVoucherLines => Set<StockReturnVoucherLine>();
        public DbSet<HrEmployeeMovement> HrEmployeeMovements => Set<HrEmployeeMovement>();
        public DbSet<HrSanctionRecord> HrSanctionRecords => Set<HrSanctionRecord>();
        public DbSet<HrOutRequest> HrOutRequests => Set<HrOutRequest>();
        public DbSet<HrLeaveType> HrLeaveTypes => Set<HrLeaveType>();
        public DbSet<HrLeaveRequest> HrLeaveRequests => Set<HrLeaveRequest>();
        public DbSet<HrLeaveBalance> HrLeaveBalances => Set<HrLeaveBalance>();
        public DbSet<HrPerformanceEvaluation> HrPerformanceEvaluations => Set<HrPerformanceEvaluation>();
        public DbSet<StockDisposalVoucher> StockDisposalVouchers => Set<StockDisposalVoucher>();
        public DbSet<StockDisposalVoucherLine> StockDisposalVoucherLines => Set<StockDisposalVoucherLine>();
        public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
        public DbSet<BeneficiaryFamilyMember> BeneficiaryFamilyMembers => Set<BeneficiaryFamilyMember>();
        public DbSet<BeneficiaryDocument> BeneficiaryDocuments => Set<BeneficiaryDocument>();
        public DbSet<BeneficiaryAssessment> BeneficiaryAssessments => Set<BeneficiaryAssessment>();
        public DbSet<BeneficiaryCommitteeDecision> BeneficiaryCommitteeDecisions => Set<BeneficiaryCommitteeDecision>();
        public DbSet<BeneficiaryOldRecord> BeneficiaryOldRecords => Set<BeneficiaryOldRecord>();
        public DbSet<ProjectTeamMember> CharityProjectTeamMembers => Set<ProjectTeamMember>();
        public DbSet<ProjectTeamMemberAttachment> CharityProjectTeamMemberAttachments => Set<ProjectTeamMemberAttachment>();

        public DbSet<BeneficiaryAidRequest> BeneficiaryAidRequests => Set<BeneficiaryAidRequest>();
        public DbSet<BeneficiaryAidRequestLine> BeneficiaryAidRequestLines => Set<BeneficiaryAidRequestLine>();
        public DbSet<BeneficiaryAidDisbursement> BeneficiaryAidDisbursements => Set<BeneficiaryAidDisbursement>();
        /// </summary>
        public DbSet<DhcpRecord> DhcpRecords => Set<DhcpRecord>();
        public DbSet<Notification2> Notifications => Set<Notification2>();

        public DbSet<KanbanTask> kanbanTasks => Set<KanbanTask>();
        public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();
        public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        public DbSet<Donor> CharityDonors => Set<Donor>();
        public DbSet<Donation> CharityDonations => Set<Donation>();
        public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
        public DbSet<SalesInvoiceLine> SalesInvoiceLines => Set<SalesInvoiceLine>();
        public DbSet<FinancialAccount> Accounts => Set<FinancialAccount>();
        public DbSet<AccountingPostingProfile> AccountingPostingProfiles => Set<AccountingPostingProfile>();
        public DbSet<BoardUser> boardUsers => Set<BoardUser>();
        public DbSet<HrEmployeeContract> HrEmployeeContracts => Set<HrEmployeeContract>();
        public DbSet<HrEmployeeFundingAssignment> HrEmployeeFundingAssignments => Set<HrEmployeeFundingAssignment>();
        public DbSet<HrEmployeeTaskAssignment> HrEmployeeTaskAssignments => Set<HrEmployeeTaskAssignment>();
        public DbSet<HrEmployeeBonus> HrEmployeeBonuses => Set<HrEmployeeBonus>();
        public DbSet<TaskAudit> TaskAudits => Set<TaskAudit>();
        public DbSet<ChatMessage2> ChatMessages => Set<ChatMessage2>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
        public DbSet<BoardDecisionAttachment> CharityBoardDecisionAttachments { get; set; }
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<CalendarAttendee> CalendarAttendees => Set<CalendarAttendee>();
        public DbSet<CharityProject> CharityProjects => Set<CharityProject>();
        public DbSet<ProjectBudgetLine> CharityProjectBudgetLines => Set<ProjectBudgetLine>();
        public DbSet<ProjectActivity> CharityProjectActivities => Set<ProjectActivity>();
        public DbSet<ProjectBeneficiary> CharityProjectBeneficiaries => Set<ProjectBeneficiary>();
        public DbSet<ProjectGrant> CharityProjectGrants => Set<ProjectGrant>();
        public DbSet<EventReminder> EventReminders => Set<EventReminder>();
        public DbSet<ADSL_Line> ADSL_Line { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Add_Receipt> Add_Receipt { get; set; }
        public DbSet<Add_product> Add_product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<BackgroundJob> BackgroundJob { get; set; }
        public DbSet<Balance> Balance { get; set; }
        public DbSet<Bill_Product> Bill_Product { get; set; }
        public DbSet<CustomerClient> Customers => Set<CustomerClient>();
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        public DbSet<Category> Category { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CustomerReceipt> CustomerReceipts => Set<CustomerReceipt>();
        //public DbSet<Commentitem> Commentitem { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Complain> Complain { get; set; }
        public DbSet<Cost> Cost { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<DebitTrans> DebitTrans { get; set; }
        public DbSet<Deliverable> Deliverable { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DomainUser> DomainUser { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Error> Error { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventItem> EventItem { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<GPS_Tracking_Car> GPS_Tracking_Car { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<IPAdress> IPAdress { get; set; }
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Importer> Importer { get; set; }
        public DbSet<Income> Income { get; set; }
        // public DbSet<Instruction> Instruction { get; set; }
        public DbSet<InternetLeasedLine> InternetLeasedLine { get; set; }
        public DbSet<Limit> Limit { get; set; }
        public DbSet<Message> Message { get; set; }
        //public DbSet<Messageitem> Messageitem { get; set; }
        public DbSet<Milestone> Milestone { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Objective> Objective { get; set; }
        public DbSet<Obstackle> Obstackle { get; set; }
        public DbSet<Onlineuser> Onlineuser { get; set; }
        public DbSet<PayedTrans> PayedTrans { get; set; }
        public DbSet<PersonalInformation> PersonalInformation { get; set; }
        public DbSet<Post> Post { get; set; }
        // Infrastructure/Persistence/AppDbContext.cs
        public DbSet<TaskBoard> TaskBoards => Set<TaskBoard>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        //public DbSet<PostItem> PostItem { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Purchases_Replace> Purchases_Replace { get; set; }
        public DbSet<Purchses_Bill> Purchses_Bill { get; set; }
        public DbSet<Receiver> Receiver { get; set; }
        public DbSet<Remove_Product> Remove_Product { get; set; }
        public DbSet<Remove_Receipt> Remove_Receipt { get; set; }
        public DbSet<Replace_Product> Replace_Product { get; set; }
        public DbSet<Requests> Requests { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<InfrastructureManagmentCore.Domains.Complains.Task> Task { get; set; }
        public DbSet<InfrastructureManagmentCore.Domains.Complains.Instruction> instructions { get; set; }
        public DbSet<Hour> hours { get; set; }

        public DbSet<SalesBill_Product> SalesBill_Product { get; set; }
        public DbSet<SalesReplace_Product> SalesReplace_Product { get; set; }
        public DbSet<Sales_Bill> Sales_Bill { get; set; }
        public DbSet<Sales_Replace> Sales_Replace { get; set; }
        public DbSet<Sender> Sender { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Stocktaking> Stocktaking { get; set; }
        public DbSet<SystemComponent> SystemComponent { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TeamMember> TeamMember { get; set; }
        public DbSet<Todo> Todo { get; set; }
        public DbSet<Transaction2> Transaction2 { get; set; }
        public DbSet<Treasury> Treasury { get; set; }
        public DbSet<TroubleTicket> TroubleTicket { get; set; }
        public DbSet<USB_Modem> USB_Modem { get; set; }
        public DbSet<Unity> Unity { get; set; }
        // public DbSet<User> User { get; set; }
        public DbSet<UserLog> UserLog { get; set; }
        public DbSet<VPNLeasedLine> VPNLeasedLine { get; set; }
        public DbSet<address> address { get; set; }
        public DbSet<connection> connection { get; set; }
        public DbSet<exhib> exhib { get; set; }
        public DbSet<exhib_product> exhib_product { get; set; }
        public DbSet<identity_products> identity_products { get; set; }
        public DbSet<invent> invent { get; set; }
        public DbSet<invent_product> invent_product { get; set; }
        public DbSet<miss_products> miss_products { get; set; }
        public DbSet<missingitem> missingitem { get; set; }
        public DbSet<portfolio> portfolio { get; set; }
        public DbSet<portif> portif { get; set; }

    }
}
