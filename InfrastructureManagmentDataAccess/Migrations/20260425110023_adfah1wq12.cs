using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adfah1wq12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingCostCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CostCenterNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParentCostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsProjectRelated = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingCostCenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingCostCenters_AccountingCostCenters_ParentCostCenterId",
                        column: x => x.ParentCostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingFiscalPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PeriodNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PeriodNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingFiscalPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountingIntegrationSourceDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModuleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDynamicPostingEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EntityClrTypeName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DatePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AmountPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumberPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TitlePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DescriptionTemplate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FinancialAccountIdPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProjectIdPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CostCenterIdPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EventCodePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DonationTypePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetingScopeCodePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurposeNamePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AidTypeIdPropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StoreMovementTypePropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingIntegrationSourceDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ADSL_Line",
                columns: table => new
                {
                    ADSLid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orderid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    speed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    package = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deliverableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADSL_Line", x => x.ADSLid);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundJob",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestbody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestType = table.Column<short>(type: "smallint", nullable: false),
                    requestheader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestmethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    requesturl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responsBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responseheader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Balance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    balance = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardMeetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Agenda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreparedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardMeetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityFunders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FunderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityFunders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityGenders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityGenders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityGovernorates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityGovernorates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrJobTitles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SystemRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrJobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    GraceMinutes = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityKafalaSponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SponsorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalIdOrTaxNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityKafalaSponsors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityMaritalStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityMaritalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityPayrollMonths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityPayrollMonths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DonorName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProjectLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    RequestedBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationYear = table.Column<int>(type: "int", nullable: true),
                    Vision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpertiseSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeesCount = table.Column<int>(type: "int", nullable: true),
                    VolunteersCount = table.Column<int>(type: "int", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProblemBackground = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProblemAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalAlignment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposedApproach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposedSolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RisksAndExternalFactors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutiveSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralGoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreparatoryRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImplementationTeamSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SustainabilityPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CharityProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetBeneficiariesCount = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Objectives = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Kpis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharitySalaryItemDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CalculationMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsInsuranceIncluded = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharitySalaryItemDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockDisposalVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisposalType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockDisposalVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockNeedRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedPurchaseInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockNeedRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockReturnVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockReturnVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PreferredArea = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AvailabilityNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteerSkillDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteerSkillDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    ChatID = table.Column<int>(name: "Chat ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    starttime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    endtime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.ChatID);
                });

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    companyid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    industry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revenue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    street2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Personalid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.companyid);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiptHeaderText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptFooterText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "connection",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Servername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    iso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nicename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    iso3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phonecode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(name: "Department ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(name: "Department Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    test = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "DhcpRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MAC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DhcpRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdObjectId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Upn = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistinguishedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FetchedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserWorkstations = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainUser",
                columns: table => new
                {
                    userid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstOU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondOU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThirdOU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    logonname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<int>(type: "int", nullable: true),
                    DomainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComputerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    printerconnected = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loginid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    group = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainUser", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "Error",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorCode = table.Column<short>(type: "smallint", nullable: false),
                    objID = table.Column<long>(type: "bigint", nullable: true),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Error", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventItem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startday = table.Column<int>(type: "int", nullable: false),
                    startmonth = table.Column<int>(type: "int", nullable: false),
                    startyear = table.Column<int>(type: "int", nullable: false),
                    starthour = table.Column<int>(type: "int", nullable: false),
                    startmin = table.Column<int>(type: "int", nullable: false),
                    endday = table.Column<int>(type: "int", nullable: false),
                    endmonth = table.Column<int>(type: "int", nullable: false),
                    endyear = table.Column<int>(type: "int", nullable: false),
                    endhour = table.Column<int>(type: "int", nullable: false),
                    endmin = table.Column<int>(type: "int", nullable: false),
                    AllDays = table.Column<bool>(type: "bit", nullable: false),
                    className = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    icon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventItem", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinacialAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ParentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    IsPosting = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequiresProject = table.Column<bool>(type: "bit", nullable: false),
                    RequiresCostCenter = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinacialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinacialAccounts_FinacialAccounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GPS_Tracking_Car",
                columns: table => new
                {
                    Carid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    carbnumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    linenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    puk1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMEI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPS_Tracking_Car", x => x.Carid);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "InternetLeasedLine",
                columns: table => new
                {
                    lineid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orderid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    speed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    package = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternetLeasedLine", x => x.lineid);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GroupNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GroupNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemGroups_ItemGroups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "ItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kanbanTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kanbanTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Limit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    limit = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Onlineuser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connectionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Onlineuser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MethodCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MethodNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MethodNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsCash = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "portfolio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domainuser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portfolio", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Receiver",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telephonenum = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receiver", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    querystring = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startReqTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endReqTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleDescrip = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Statusid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Statusid);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskAudits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskBoards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Team_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.TeamID);
                });

            migrationBuilder.CreateTable(
                name: "Treasury",
                columns: table => new
                {
                    TreasuryID = table.Column<int>(name: "Treasury ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treasury", x => x.TreasuryID);
                });

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    TypeID = table.Column<int>(name: "Type ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(name: "Type Name", type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    hhdsd = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USB_Modem",
                columns: table => new
                {
                    USBMid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    puk = table.Column<double>(type: "float", nullable: false),
                    pin = table.Column<double>(type: "float", nullable: false),
                    SKU = table.Column<double>(type: "float", nullable: false),
                    SIM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMEI = table.Column<double>(type: "float", nullable: false),
                    SN = table.Column<double>(type: "float", nullable: false),
                    OrderID = table.Column<double>(type: "float", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliverableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USB_Modem", x => x.USBMid);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VPNLeasedLine",
                columns: table => new
                {
                    lineid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orderid = table.Column<double>(type: "float", nullable: false),
                    speed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    package = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VPNLeasedLine", x => x.lineid);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WarehouseNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssignedRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssignedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountingJournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FiscalPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedJournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingJournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingJournalEntries_AccountingFiscalPeriods_FiscalPeriodId",
                        column: x => x.FiscalPeriodId,
                        principalTable: "AccountingFiscalPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardMeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DecisionKind = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsibleParty = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardDecisions_CharityBoardMeetings_BoardMeetingId",
                        column: x => x.BoardMeetingId,
                        principalTable: "CharityBoardMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardMeetingAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardMeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttachmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardMeetingAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardMeetingAttachments_CharityBoardMeetings_BoardMeetingId",
                        column: x => x.BoardMeetingId,
                        principalTable: "CharityBoardMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardMeetingAttendees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardMeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PositionTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AttendanceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardMeetingAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardMeetingAttendees_CharityBoardMeetings_BoardMeetingId",
                        column: x => x.BoardMeetingId,
                        principalTable: "CharityBoardMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardMeetingMinutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardMeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegalOpeningText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscussionSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendationsSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalClosingText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullMinuteText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardMeetingMinutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardMeetingMinutes_CharityBoardMeetings_BoardMeetingId",
                        column: x => x.BoardMeetingId,
                        principalTable: "CharityBoardMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityGrantAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgreementNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FunderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentTerms = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReportingRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityGrantAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityGrantAgreements_CharityFunders_FunderId",
                        column: x => x.FunderId,
                        principalTable: "CharityFunders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityCities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GovernorateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityCities_CharityGovernorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "CharityGovernorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployees_CharityHrDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "CharityHrDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployees_CharityHrJobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "CharityHrJobTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalActivitys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsibleRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeededResources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlannedStartMonth = table.Column<int>(type: "int", nullable: true),
                    PlannedEndMonth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalActivitys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalActivitys_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalAttachments_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalMonitoringIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Indicator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportingFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalMonitoringIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalMonitoringIndicators_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalObjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectiveType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalObjectives_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalPastExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Donor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TargetGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultAchieved = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalPastExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalPastExperiences_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalTargetGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetCount = table.Column<int>(type: "int", nullable: true),
                    AgeRange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenderNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectionCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BenefitDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalTargetGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalTargetGroups_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalTeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalTeamMembers_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalWorkPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsible = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartMonth = table.Column<int>(type: "int", nullable: true),
                    EndMonth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalWorkPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalWorkPlans_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectActivities_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectBudgetLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LineType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlannedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectBudgetLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectBudgetLines_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SuccessIndicator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AchievedValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectGoals_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectPhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsiblePersonName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectPhases_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockDisposalVoucherLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockDisposalVoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockDisposalVoucherLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStockDisposalVoucherLines_CharityStockDisposalVouchers_StockDisposalVoucherId",
                        column: x => x.StockDisposalVoucherId,
                        principalTable: "CharityStockDisposalVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockNeedRequestLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockNeedRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FulfilledQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockNeedRequestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStockNeedRequestLines_CharityStockNeedRequests_StockNeedRequestId",
                        column: x => x.StockNeedRequestId,
                        principalTable: "CharityStockNeedRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityStockReturnVoucherLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockReturnVoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStockReturnVoucherLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStockReturnVoucherLines_CharityStockReturnVouchers_StockReturnVoucherId",
                        column: x => x.StockReturnVoucherId,
                        principalTable: "CharityStockReturnVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteerAvailabilitySlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeekName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ToTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    AvailabilityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsRemoteAllowed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteerAvailabilitySlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityVolunteerAvailabilitySlots_CharityVolunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "CharityVolunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteerProjectAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RoleTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AssignmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TargetHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteerProjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityVolunteerProjectAssignments_CharityVolunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "CharityVolunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteerSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteerSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityVolunteerSkills_CharityVolunteerSkillDefinitions_SkillDefinitionId",
                        column: x => x.SkillDefinitionId,
                        principalTable: "CharityVolunteerSkillDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityVolunteerSkills_CharityVolunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "CharityVolunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployerID = table.Column<int>(name: "Employer ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(name: "Department Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(name: "First Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(name: "Last Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalID = table.Column<string>(name: "National ID", type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HourSalary = table.Column<double>(name: "Hour Salary", type: "float", nullable: true),
                    Salary = table.Column<double>(type: "float", nullable: true),
                    Funds = table.Column<double>(type: "float", nullable: true),
                    Penalty = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    Payed = table.Column<double>(type: "float", nullable: true),
                    Remained = table.Column<double>(type: "float", nullable: true),
                    Companyid = table.Column<int>(type: "int", nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployerID);
                    table.ForeignKey(
                        name: "FK_Employee_Company_Companyid",
                        column: x => x.Companyid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    initialBeginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationPeriod = table.Column<TimeSpan>(type: "time", nullable: false),
                    TokenString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    connectionID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Session_connection_connectionID",
                        column: x => x.connectionID,
                        principalTable: "connection",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccountTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAccountTransactions_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOldRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOldRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOldRecords_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescriptionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RightSph = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    RightCyl = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    RightAxis = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LeftSph = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LeftCyl = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LeftAxis = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    AddValue = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    IPD = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    SHeight = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DirectoryUserId = table.Column<int>(type: "int", nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MacAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FetchedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectoryDevices_DirectoryUsers_DirectoryUserId",
                        column: x => x.DirectoryUserId,
                        principalTable: "DirectoryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountingIntegrationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EventNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProfileNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MatchDonationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatchTargetingScopeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MatchPurposeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MatchAidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MatchStoreMovementType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    DebitAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UseSourceFinancialAccountAsDebit = table.Column<bool>(type: "bit", nullable: false),
                    UseSourceFinancialAccountAsCredit = table.Column<bool>(type: "bit", nullable: false),
                    DefaultCostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutoPost = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingIntegrationProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingIntegrationProfiles_AccountingCostCenters_DefaultCostCenterId",
                        column: x => x.DefaultCostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingIntegrationProfiles_FinacialAccounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingIntegrationProfiles_FinacialAccounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountingPostingProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ModuleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EventCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DonationType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    TargetingScopeCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PurposeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DebitAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UseSourceFinancialAccountAsDebit = table.Column<bool>(type: "bit", nullable: false),
                    UseSourceFinancialAccountAsCredit = table.Column<bool>(type: "bit", nullable: false),
                    DefaultCostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutoPost = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPostingProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingPostingProfiles_AccountingCostCenters_DefaultCostCenterId",
                        column: x => x.DefaultCostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingPostingProfiles_FinacialAccounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingPostingProfiles_FinacialAccounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectAccountingProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultCostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultRevenueAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultExpenseAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutoTagJournalLinesWithProject = table.Column<bool>(type: "bit", nullable: false),
                    AutoUseProjectCostCenter = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAccountingProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAccountingProfiles_AccountingCostCenters_DefaultCostCenterId",
                        column: x => x.DefaultCostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAccountingProfiles_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAccountingProfiles_FinacialAccounts_DefaultExpenseAccountId",
                        column: x => x.DefaultExpenseAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAccountingProfiles_FinacialAccounts_DefaultRevenueAccountId",
                        column: x => x.DefaultRevenueAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LimitID = table.Column<long>(type: "bigint", nullable: true),
                    balanceID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Balance_balanceID",
                        column: x => x.balanceID,
                        principalTable: "Balance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Category_Limit_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiptDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpenseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpenseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPosted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "boardUsers",
                columns: table => new
                {
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardUsers", x => new { x.BoardId, x.UserId });
                    table.ForeignKey(
                        name: "FK_boardUsers_TaskBoards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "TaskBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "ToDo"),
                    Priority = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "Medium"),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_TaskBoards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "TaskBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpticalItemType = table.Column<int>(type: "int", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EyeSize = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    BridgeSize = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    TempleLength = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LensMaterial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LensIndex = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LensCoating = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequiresPrescription = table.Column<bool>(type: "bit", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ItemNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsService = table.Column<bool>(type: "bit", nullable: false),
                    IsStockItem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReorderQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemGroups_ItemGroupId",
                        column: x => x.ItemGroupId,
                        principalTable: "ItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPAddress",
                columns: table => new
                {
                    IPid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    openTCPInboundports = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    openUDPInboundports = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    openTCPOutboundports = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    openUDPOutboundports = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ILineid = table.Column<int>(type: "int", nullable: true),
                    VLineid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPAddress", x => x.IPid);
                    table.ForeignKey(
                        name: "FK_IPAddress_InternetLeasedLine_ILineid",
                        column: x => x.ILineid,
                        principalTable: "InternetLeasedLine",
                        principalColumn: "lineid");
                    table.ForeignKey(
                        name: "FK_IPAddress_VPNLeasedLine_VLineid",
                        column: x => x.VLineid,
                        principalTable: "VPNLeasedLine",
                        principalColumn: "lineid");
                });

            migrationBuilder.CreateTable(
                name: "SystemComponent",
                columns: table => new
                {
                    SyscomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GPSid = table.Column<int>(type: "int", nullable: true),
                    userid = table.Column<int>(type: "int", nullable: true),
                    ADSlid = table.Column<int>(type: "int", nullable: true),
                    Intenetlineid = table.Column<int>(type: "int", nullable: true),
                    VPNlineid = table.Column<int>(type: "int", nullable: true),
                    USBMid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemComponent", x => x.SyscomId);
                    table.ForeignKey(
                        name: "FK_SystemComponent_ADSL_Line_ADSlid",
                        column: x => x.ADSlid,
                        principalTable: "ADSL_Line",
                        principalColumn: "ADSLid");
                    table.ForeignKey(
                        name: "FK_SystemComponent_DomainUser_userid",
                        column: x => x.userid,
                        principalTable: "DomainUser",
                        principalColumn: "userid");
                    table.ForeignKey(
                        name: "FK_SystemComponent_GPS_Tracking_Car_GPSid",
                        column: x => x.GPSid,
                        principalTable: "GPS_Tracking_Car",
                        principalColumn: "Carid");
                    table.ForeignKey(
                        name: "FK_SystemComponent_InternetLeasedLine_Intenetlineid",
                        column: x => x.Intenetlineid,
                        principalTable: "InternetLeasedLine",
                        principalColumn: "lineid");
                    table.ForeignKey(
                        name: "FK_SystemComponent_USB_Modem_USBMid",
                        column: x => x.USBMid,
                        principalTable: "USB_Modem",
                        principalColumn: "USBMid");
                    table.ForeignKey(
                        name: "FK_SystemComponent_VPNLeasedLine_VPNlineid",
                        column: x => x.VPNlineid,
                        principalTable: "VPNLeasedLine",
                        principalColumn: "lineid");
                });

            migrationBuilder.CreateTable(
                name: "CharityStoreReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStoreReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStoreReceipts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosHolds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoldNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoldDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosHolds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosHolds_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosHolds_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SupplierInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StockNeedRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingJournalEntryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingJournalEntryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingJournalEntryLines_AccountingCostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingJournalEntryLines_AccountingJournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "AccountingJournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountingJournalEntryLines_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardDecisionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardDecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardDecisionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardDecisionAttachments_CharityBoardDecisions_BoardDecisionId",
                        column: x => x.BoardDecisionId,
                        principalTable: "CharityBoardDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBoardDecisionFollowUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardDecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBoardDecisionFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBoardDecisionFollowUps_CharityBoardDecisions_BoardDecisionId",
                        column: x => x.BoardDecisionId,
                        principalTable: "CharityBoardDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityGrantConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConditionTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ConditionDetails = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFulfilled = table.Column<bool>(type: "bit", nullable: false),
                    FulfilledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityGrantConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityGrantConditions_CharityGrantAgreements_GrantAgreementId",
                        column: x => x.GrantAgreementId,
                        principalTable: "CharityGrantAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityGrantInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityGrantInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityGrantInstallments_CharityGrantAgreements_GrantAgreementId",
                        column: x => x.GrantAgreementId,
                        principalTable: "CharityGrantAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityGrantInstallments_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityGrantInstallments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectGrants_CharityGrantAgreements_GrantAgreementId",
                        column: x => x.GrantAgreementId,
                        principalTable: "CharityGrantAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityProjectGrants_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityAreas_CharityCities_CityId",
                        column: x => x.CityId,
                        principalTable: "CharityCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityEmployeeSalaryStructures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalaryItemDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityEmployeeSalaryStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityEmployeeSalaryStructures_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityEmployeeSalaryStructures_CharitySalaryItemDefinitions_SalaryItemDefinitionId",
                        column: x => x.SalaryItemDefinitionId,
                        principalTable: "CharitySalaryItemDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateMinutes = table.Column<int>(type: "int", nullable: false),
                    EarlyLeaveMinutes = table.Column<int>(type: "int", nullable: false),
                    OvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrAttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrAttendanceRecords_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityHrAttendanceRecords_CharityHrShifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "CharityHrShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrEmployeeBonuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BonusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BonusType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayrollMonthId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrEmployeeBonuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployeeBonuses_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrEmployeeContracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FundingNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrEmployeeContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployeeContracts_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrEmployeeFundingAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FunderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FundingSourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CharityProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrantOrBudgetLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrEmployeeFundingAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployeeFundingAssignments_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityHrEmployeeTaskAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharityProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaskTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimaryDuty = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityHrEmployeeTaskAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityHrEmployeeTaskAssignments_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityPayrollEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollMonthId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttendanceDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Additions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityPayrollEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityPayrollEmployees_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityPayrollEmployees_CharityPayrollMonths_PayrollMonthId",
                        column: x => x.PayrollMonthId,
                        principalTable: "CharityPayrollMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HrEmployeeMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromJobTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToJobTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecisionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployeeMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployeeMovements_CharityHrDepartments_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "CharityHrDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeMovements_CharityHrDepartments_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "CharityHrDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeMovements_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrEmployeeMovements_CharityHrJobTitles_FromJobTitleId",
                        column: x => x.FromJobTitleId,
                        principalTable: "CharityHrJobTitles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeMovements_CharityHrJobTitles_ToJobTitleId",
                        column: x => x.ToJobTitleId,
                        principalTable: "CharityHrJobTitles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrOutRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ToTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrOutRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrOutRequests_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HrPerformanceEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EvaluationPeriod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisciplineScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PerformanceScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CooperationScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InitiativeScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TotalScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPerformanceEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPerformanceEvaluations_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrPerformanceEvaluations_CharityHrEmployees_EvaluatorEmployeeId",
                        column: x => x.EvaluatorEmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrSanctionRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanctionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SanctionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSanctionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSanctionRecords_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectSubGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SuccessIndicator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AchievedValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectSubGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectSubGoals_CharityProjectGoals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "CharityProjectGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectPhaseActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlannedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResponsiblePersonName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectPhaseActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectPhaseActivities_CharityProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectPhaseMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectPhaseMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectPhaseMilestones_CharityProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectTrackingLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RequiresAttention = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectTrackingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectTrackingLogs_CharityProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CharityProjectTrackingLogs_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharityVolunteerHourLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityVolunteerHourLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityVolunteerHourLogs_CharityVolunteerProjectAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "CharityVolunteerProjectAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityVolunteerHourLogs_CharityVolunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "CharityVolunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountID = table.Column<int>(name: "Account ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: true),
                    TotalExpenses = table.Column<double>(type: "float", nullable: true),
                    TotalIncome = table.Column<double>(type: "float", nullable: true),
                    PreviousBalalnce = table.Column<double>(type: "float", nullable: true),
                    FirstBalance = table.Column<double>(type: "float", nullable: true),
                    FinalBalance = table.Column<double>(type: "float", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreasuryId = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_Account_Employee_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Account_Treasury_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "Treasury",
                        principalColumn: "Treasury ID");
                });

            migrationBuilder.CreateTable(
                name: "portif",
                columns: table => new
                {
                    portfilioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    companyid = table.Column<int>(type: "int", nullable: true),
                    Employer_ID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Trial = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    Accounting = table.Column<int>(type: "int", nullable: true),
                    Finincial = table.Column<int>(type: "int", nullable: true),
                    System = table.Column<int>(type: "int", nullable: true),
                    Chat = table.Column<int>(type: "int", nullable: true),
                    Event = table.Column<int>(type: "int", nullable: true),
                    Graphs = table.Column<int>(type: "int", nullable: true),
                    Social = table.Column<int>(type: "int", nullable: true),
                    Project = table.Column<int>(type: "int", nullable: true),
                    CProject = table.Column<int>(type: "int", nullable: true),
                    Notification = table.Column<int>(type: "int", nullable: true),
                    Todo = table.Column<int>(type: "int", nullable: true),
                    Complain = table.Column<int>(type: "int", nullable: true),
                    TT = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portif", x => x.portfilioId);
                    table.ForeignKey(
                        name: "FK_portif_Company_companyid",
                        column: x => x.companyid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_portif_Employee_Employer_ID",
                        column: x => x.Employer_ID,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_portif_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "SalesInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sender",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimitID = table.Column<long>(type: "bigint", nullable: true),
                    balanceID = table.Column<long>(type: "bigint", nullable: true),
                    CatId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sender", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sender_Balance_balanceID",
                        column: x => x.balanceID,
                        principalTable: "Balance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sender_Category_CatId",
                        column: x => x.CatId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sender_Limit_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectExpenseLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectBudgetLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExpenseLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectExpenseLinks_AccountingCostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExpenseLinks_CharityProjectBudgetLines_ProjectBudgetLineId",
                        column: x => x.ProjectBudgetLineId,
                        principalTable: "CharityProjectBudgetLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExpenseLinks_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectExpenseLinks_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhaseExpenseLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectBudgetLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncludeInActualCost = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhaseExpenseLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseExpenseLinks_AccountingCostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseExpenseLinks_CharityProjectBudgetLines_ProjectBudgetLineId",
                        column: x => x.ProjectBudgetLineId,
                        principalTable: "CharityProjectBudgetLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseExpenseLinks_CharityProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseExpenseLinks_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseExpenseLinks_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemWarehouseBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemWarehouseBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemWarehouseBalances_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemWarehouseBalances_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RelatedWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Warehouses_RelatedWarehouseId",
                        column: x => x.RelatedWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityStoreReceiptLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStoreReceiptLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStoreReceiptLines_CharityStoreReceipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "CharityStoreReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityStoreReceiptLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosHoldLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosHoldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosHoldLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosHoldLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosHoldLines_PosHolds_PosHoldId",
                        column: x => x.PosHoldId,
                        principalTable: "PosHolds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceLines_PurchaseInvoices_PurchaseInvoiceId",
                        column: x => x.PurchaseInvoiceId,
                        principalTable: "PurchaseInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityDonors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DonorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NationalIdOrTaxNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GovernorateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreferredCommunicationMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDonors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityDonors_CharityAreas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "CharityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonors_CharityCities_CityId",
                        column: x => x.CityId,
                        principalTable: "CharityCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonors_CharityGovernorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "CharityGovernorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityPayrollEmployeeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalaryItemDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityPayrollEmployeeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityPayrollEmployeeItems_CharityPayrollEmployees_PayrollEmployeeId",
                        column: x => x.PayrollEmployeeId,
                        principalTable: "CharityPayrollEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityPayrollEmployeeItems_CharitySalaryItemDefinitions_SalaryItemDefinitionId",
                        column: x => x.SalaryItemDefinitionId,
                        principalTable: "CharitySalaryItemDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharityPayrollPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityPayrollPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityPayrollPayments_CharityPayrollEmployees_PayrollEmployeeId",
                        column: x => x.PayrollEmployeeId,
                        principalTable: "CharityPayrollEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityPayrollPayments_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CharityPayrollPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectSubGoalActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponsiblePersonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlannedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TargetGroup = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlannedDurationDays = table.Column<int>(type: "int", nullable: false),
                    PerformanceIndicator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VerificationMeans = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetAchievement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantityUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlannedHoursPerDay = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TargetGroupDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectSubGoalActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectSubGoalActivities_CharityProjectSubGoals_SubGoalId",
                        column: x => x.SubGoalId,
                        principalTable: "CharityProjectSubGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectPhaseTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PercentComplete = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpentHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AssignedToName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RequiresDailyFollowUp = table.Column<bool>(type: "bit", nullable: false),
                    LastDailyUpdateAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectPhaseTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectPhaseTasks_CharityProjectPhaseActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "CharityProjectPhaseActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityProjectPhaseTasks_CharityProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DebitTrans",
                columns: table => new
                {
                    DebitID = table.Column<int>(name: "Debit ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreasuryId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitTrans", x => x.DebitID);
                    table.ForeignKey(
                        name: "FK_DebitTrans_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_DebitTrans_Employee_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_DebitTrans_Treasury_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "Treasury",
                        principalColumn: "Treasury ID");
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseID = table.Column<int>(name: "Expense ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreasuryId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseID);
                    table.ForeignKey(
                        name: "FK_Expense_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_Expense_Employee_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Expense_Treasury_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "Treasury",
                        principalColumn: "Treasury ID");
                });

            migrationBuilder.CreateTable(
                name: "Income",
                columns: table => new
                {
                    ExpenseID = table.Column<int>(name: "Expense ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employeeid = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreasuryId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Income", x => x.ExpenseID);
                    table.ForeignKey(
                        name: "FK_Income_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_Income_Employee_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Income_Treasury_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "Treasury",
                        principalColumn: "Treasury ID");
                });

            migrationBuilder.CreateTable(
                name: "PayedTrans",
                columns: table => new
                {
                    PayedID = table.Column<int>(name: "Payed ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreasuryId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayedTrans", x => x.PayedID);
                    table.ForeignKey(
                        name: "FK_PayedTrans_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_PayedTrans_Employee_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_PayedTrans_Treasury_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "Treasury",
                        principalColumn: "Treasury ID");
                });

            migrationBuilder.CreateTable(
                name: "OpticalWorkOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadyDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false),
                    FrameNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LensNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WorkshopNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DeliveryNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpticalWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpticalWorkOrders_CustomerClients_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpticalWorkOrders_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpticalWorkOrders_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesInvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceLines_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesInvoicePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoicePayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoicePayments_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReturnDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalSalesInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturnInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesReturnInvoices_SalesInvoices_OriginalSalesInvoiceId",
                        column: x => x.OriginalSalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturnInvoices_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    header_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    messageId = table.Column<long>(type: "bigint", nullable: false),
                    SenderID = table.Column<long>(type: "bigint", nullable: true),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: true),
                    AccountNumber = table.Column<long>(type: "bigint", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorCode = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Category_CatId",
                        column: x => x.CatId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Message_Receiver_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Receiver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Message_Sender_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Sender",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharityActivityPhaseAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContributionPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlannedDurationDays = table.Column<int>(type: "int", nullable: true),
                    ActualDurationDays = table.Column<int>(type: "int", nullable: true),
                    PlannedHoursPerDay = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityActivityPhaseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityActivityPhaseAssignments_CharityProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityActivityPhaseAssignments_CharityProjectSubGoalActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "CharityProjectSubGoalActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectTaskDailyUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HoursSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectTaskDailyUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectTaskDailyUpdates_CharityProjectPhaseTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "CharityProjectPhaseTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesReturnInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalSalesInvoiceLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturnLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesReturnLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturnLines_SalesInvoiceLines_OriginalSalesInvoiceLineId",
                        column: x => x.OriginalSalesInvoiceLineId,
                        principalTable: "SalesInvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturnLines_SalesReturnInvoices_SalesReturnInvoiceId",
                        column: x => x.SalesReturnInvoiceId,
                        principalTable: "SalesReturnInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Add product",
                columns: table => new
                {
                    addid = table.Column<int>(name: "add id", type: "int", nullable: true),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddProid = table.Column<int>(name: "AddPro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Add product", x => x.AddProid);
                    table.ForeignKey(
                        name: "FK_Add product_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Add product_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Add product_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "Add Receipt",
                columns: table => new
                {
                    addid = table.Column<int>(name: "add id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: true),
                    billid = table.Column<int>(name: "bill id", type: "int", nullable: true),
                    Deliveredto = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: true),
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<double>(type: "float", nullable: true),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true),
                    CustomerCustomter_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Add Receipt", x => x.addid);
                    table.ForeignKey(
                        name: "FK_Add Receipt_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Add Receipt_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Add Receipt_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    addressid = table.Column<int>(name: "address id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    addressLine = table.Column<string>(name: "address Line", type: "text", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.addressid);
                    table.ForeignKey(
                        name: "FK_address_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_address_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_address_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Customerid = table.Column<int>(name: "Customer id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    addressid = table.Column<int>(name: "address id", type: "int", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    payed = table.Column<double>(type: "float", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    remined = table.Column<double>(type: "float", nullable: false),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    Account_id = table.Column<int>(type: "int", nullable: true),
                    comid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Customerid);
                    table.ForeignKey(
                        name: "FK_Customer_Account_Account_id",
                        column: x => x.Account_id,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_Customer_Company_comid",
                        column: x => x.comid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Customer_address_address id",
                        column: x => x.addressid,
                        principalTable: "address",
                        principalColumn: "address id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Importer",
                columns: table => new
                {
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    addressid = table.Column<int>(name: "address id", type: "int", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    payed = table.Column<double>(type: "float", nullable: false),
                    remined = table.Column<double>(type: "float", nullable: false),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    Account_id = table.Column<int>(type: "int", nullable: true),
                    comid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importer", x => x.importerid);
                    table.ForeignKey(
                        name: "FK_Importer_Account_Account_id",
                        column: x => x.Account_id,
                        principalTable: "Account",
                        principalColumn: "Account ID");
                    table.ForeignKey(
                        name: "FK_Importer_Company_comid",
                        column: x => x.comid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Importer_address_address id",
                        column: x => x.addressid,
                        principalTable: "address",
                        principalColumn: "address id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales Bill",
                columns: table => new
                {
                    Billid = table.Column<int>(name: "Bill id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customerid = table.Column<int>(name: "Customer id", type: "int", nullable: true),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    code = table.Column<int>(type: "int", nullable: true),
                    discount = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: false),
                    subtotal = table.Column<double>(type: "float", nullable: false),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    comid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales Bill", x => x.Billid);
                    table.ForeignKey(
                        name: "FK_Sales Bill_Company_comid",
                        column: x => x.comid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Sales Bill_Customer_Customer id",
                        column: x => x.Customerid,
                        principalTable: "Customer",
                        principalColumn: "Customer id");
                });

            migrationBuilder.CreateTable(
                name: "Sales Replace",
                columns: table => new
                {
                    Replaceid = table.Column<int>(name: "Replace id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customerid = table.Column<int>(name: "Customer id", type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    discount = table.Column<double>(type: "float", nullable: true),
                    code = table.Column<int>(type: "int", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    subtotal = table.Column<double>(type: "float", nullable: true),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    comid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales Replace", x => x.Replaceid);
                    table.ForeignKey(
                        name: "FK_Sales Replace_Company_comid",
                        column: x => x.comid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Sales Replace_Customer_Customer id",
                        column: x => x.Customerid,
                        principalTable: "Customer",
                        principalColumn: "Customer id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "BeneficiaryContactLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ContactDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FollowUpNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryContactLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bill Products",
                columns: table => new
                {
                    BillProid = table.Column<int>(name: "BillPro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Billid = table.Column<int>(name: "Bill id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addproid = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unitprice = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill Products", x => x.BillProid);
                    table.ForeignKey(
                        name: "FK_Bill Products_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Bill Products_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Bill Products_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "CalendarAttendees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarAttendees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllDay = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinutesBefore = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventReminders_CalendarEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidCycleBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AidCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeDecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisbursedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastDisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisbursementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StopReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidCycleBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityAidCycleBeneficiaries_CharityAidTypes_AidTypeId",
                        column: x => x.AidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidCycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CycleType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PeriodYear = table.Column<int>(type: "int", nullable: true),
                    PeriodMonth = table.Column<int>(type: "int", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedDisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BeneficiariesCount = table.Column<int>(type: "int", nullable: false),
                    TotalPlannedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalDisbursedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityAidCycles_CharityAidTypes_AidTypeId",
                        column: x => x.AidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidDisbursementFundingLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountConsumed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidDisbursementFundingLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidDisbursements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AidRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrantAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Approved"),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExecutionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "FullyDisbursed"),
                    ExecutedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExecutedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidDisbursements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityAidDisbursements_CharityAidTypes_AidTypeId",
                        column: x => x.AidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityAidDisbursements_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityAidDisbursements_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityAidRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UrgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityAidRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityAidRequests_CharityAidTypes_AidTypeId",
                        column: x => x.AidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityAidRequests_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaritalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AlternatePhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GovernorateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FamilyMembersCount = table.Column<int>(type: "int", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IncomeSource = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HealthStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EducationStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    WorkStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HousingStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityAreas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "CharityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityBeneficiaryStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CharityBeneficiaryStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityCities_CityId",
                        column: x => x.CityId,
                        principalTable: "CharityCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityGenders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "CharityGenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityGovernorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "CharityGovernorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaries_CharityMaritalStatuses_MaritalStatusId",
                        column: x => x.MaritalStatusId,
                        principalTable: "CharityMaritalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryDocuments_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryFamilyMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EducationStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    WorkStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HealthCondition = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDependent = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryFamilyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryFamilyMembers_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryFamilyMembers_CharityGenders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "CharityGenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResearchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AidTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SourceOfRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResearcherCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResearcherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitteeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyMembersCount = table.Column<int>(type: "int", nullable: false),
                    TotalIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasExistingProject = table.Column<bool>(type: "bit", nullable: false),
                    ExistingProjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingProjectSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredNeedsPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredNeedsSecondary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HousingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResearcherReport = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResearchManagerOpinion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentToCommitteeAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommitteeDecidedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommitteeSentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommitteeSentByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitteeDecisionByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitteeDecisionAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommitteeDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitteeDecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchs_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityKafalaCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorshipType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MonthlyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AutoIncludeInAidCycles = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityKafalaCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityKafalaCases_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityKafalaCases_CharityKafalaSponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "CharityKafalaSponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityKafalaCases_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityKafalaCases_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BenefitType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectBeneficiaries_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityProjectBeneficiaries_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityStoreIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IssuedToName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStoreIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStoreIssues_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityStoreIssues_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityStoreIssues_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchCommitteeEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeMeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedAidType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DurationMonths = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchCommitteeEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchCommitteeEvaluations_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchDebts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasLegalCase = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchDebts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchDebts_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchExpenseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchExpenseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchExpenseItems_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchFamilyMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Income = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchFamilyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchFamilyMembers_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchHouseAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Exists = table.Column<bool>(type: "bit", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchHouseAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchHouseAssets_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchIncomeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncomeType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchIncomeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchIncomeItems_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryHumanitarianResearchReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryHumanitarianResearchReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryHumanitarianResearchReviews_CharityBeneficiaryHumanitarianResearchs_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "CharityBeneficiaryHumanitarianResearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityKafalaPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KafalaCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeriodLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AidCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityKafalaPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityKafalaPayments_CharityKafalaCases_KafalaCaseId",
                        column: x => x.KafalaCaseId,
                        principalTable: "CharityKafalaCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityKafalaPayments_CharityKafalaSponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "CharityKafalaSponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityKafalaPayments_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityKafalaPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityStoreIssueLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityStoreIssueLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityStoreIssueLines_CharityStoreIssues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "CharityStoreIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityStoreIssueLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhaseStoreIssueLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncludeInActualCost = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhaseStoreIssueLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseStoreIssueLinks_AccountingCostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "AccountingCostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseStoreIssueLinks_CharityProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseStoreIssueLinks_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhaseStoreIssueLinks_CharityStoreIssues_StoreIssueId",
                        column: x => x.StoreIssueId,
                        principalTable: "CharityStoreIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResearcherUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HousingCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EconomicCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HealthCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SocialCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecommendedAidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecommendationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AssessmentScore = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RecommendationText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DecisionSuggested = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryAssessments_CharityAidTypes_RecommendedAidTypeId",
                        column: x => x.RecommendedAidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryAssessments_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryCommitteeDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApprovedAidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DurationInMonths = table.Column<int>(type: "int", nullable: true),
                    CommitteeNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryCommitteeDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryCommitteeDecisions_CharityAidTypes_ApprovedAidTypeId",
                        column: x => x.ApprovedAidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryCommitteeDecisions_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryOldRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryOldRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryOldRecords_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityDonationAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AidRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonationInKindItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllocatedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDonationAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityDonationAllocations_CharityAidRequests_AidRequestId",
                        column: x => x.AidRequestId,
                        principalTable: "CharityAidRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonationAllocations_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityDonationInKindItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedUnitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedTotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDonationInKindItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityDonationInKindItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonationInKindItems_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityDonations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AidTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRestricted = table.Column<bool>(type: "bit", nullable: false),
                    CampaignName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TargetingScopeCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false, defaultValue: "SpecificRequests"),
                    GeneralPurposeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityDonations_CharityAidTypes_AidTypeId",
                        column: x => x.AidTypeId,
                        principalTable: "CharityAidTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonations_CharityDonors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "CharityDonors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityDonations_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityDonations_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    MessageID = table.Column<int>(name: "Message ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderid = table.Column<int>(type: "int", nullable: true),
                    Chatid = table.Column<int>(type: "int", nullable: true),
                    SPersonalid = table.Column<int>(type: "int", nullable: true),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    RPersonalid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_ChatMessage_Chat_Chatid",
                        column: x => x.Chatid,
                        principalTable: "Chat",
                        principalColumn: "Chat ID");
                    table.ForeignKey(
                        name: "FK_ChatMessage_Employee_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "Employee",
                        principalColumn: "Employer ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessage_Employee_senderid",
                        column: x => x.senderid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    commentid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    todoid = table.Column<int>(type: "int", nullable: true),
                    Complainid = table.Column<int>(type: "int", nullable: true),
                    userid = table.Column<int>(type: "int", nullable: true),
                    employerid = table.Column<int>(type: "int", nullable: true),
                    Miloid = table.Column<int>(type: "int", nullable: true),
                    postid = table.Column<int>(type: "int", nullable: true),
                    Tickid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Taskid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_Comment_DomainUser_userid",
                        column: x => x.userid,
                        principalTable: "DomainUser",
                        principalColumn: "userid");
                    table.ForeignKey(
                        name: "FK_Comment_Employee_employerid",
                        column: x => x.employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                });

            migrationBuilder.CreateTable(
                name: "Complain",
                columns: table => new
                {
                    ComplainID = table.Column<int>(name: "Complain ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    importance = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lasttransaction = table.Column<int>(type: "int", nullable: false),
                    endtime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senderid = table.Column<int>(type: "int", nullable: true),
                    solverid = table.Column<int>(type: "int", nullable: true),
                    personald = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complain", x => x.ComplainID);
                    table.ForeignKey(
                        name: "FK_Complain_DomainUser_senderid",
                        column: x => x.senderid,
                        principalTable: "DomainUser",
                        principalColumn: "userid");
                    table.ForeignKey(
                        name: "FK_Complain_Employee_solverid",
                        column: x => x.solverid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                });

            migrationBuilder.CreateTable(
                name: "Cost",
                columns: table => new
                {
                    costid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priceper = table.Column<double>(type: "float", nullable: false),
                    quantity = table.Column<double>(type: "float", nullable: false),
                    totalCost = table.Column<double>(type: "float", nullable: false),
                    Milestoneid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cost", x => x.costid);
                });

            migrationBuilder.CreateTable(
                name: "Deliverable",
                columns: table => new
                {
                    DeliverableID = table.Column<int>(name: "Deliverable ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliverableName = table.Column<string>(name: "Deliverable Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MilestoneId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliverable", x => x.DeliverableID);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Docid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Miloid = table.Column<int>(type: "int", nullable: true),
                    proid = table.Column<int>(type: "int", nullable: true),
                    reqid = table.Column<int>(type: "int", nullable: true),
                    ReqResid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Docid);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllDays = table.Column<bool>(type: "bit", nullable: false),
                    className = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loginid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Event_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Event_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Event_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "exhib",
                columns: table => new
                {
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    exhib_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: true),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<double>(type: "float", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exhib", x => x.exhibid);
                    table.ForeignKey(
                        name: "FK_exhib_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_exhib_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_exhib_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_exhib_address_address_id",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "address id");
                });

            migrationBuilder.CreateTable(
                name: "exhib products",
                columns: table => new
                {
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    previousunit = table.Column<double>(type: "float", nullable: true),
                    finalunit = table.Column<double>(type: "float", nullable: true),
                    previouscartoon = table.Column<double>(type: "float", nullable: true),
                    finalcartoon = table.Column<double>(type: "float", nullable: true),
                    previousroll = table.Column<double>(type: "float", nullable: true),
                    finalroll = table.Column<double>(type: "float", nullable: true),
                    goodunit = table.Column<double>(type: "float", nullable: true),
                    goodcartoon = table.Column<double>(type: "float", nullable: true),
                    goodroll = table.Column<double>(type: "float", nullable: true),
                    badunit = table.Column<double>(type: "float", nullable: true),
                    badcartoon = table.Column<double>(type: "float", nullable: true),
                    badroll = table.Column<double>(type: "float", nullable: true),
                    usedunit = table.Column<double>(type: "float", nullable: true),
                    usedcartoon = table.Column<double>(type: "float", nullable: true),
                    usedroll = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exhib products", x => x.exhibid);
                    table.ForeignKey(
                        name: "FK_exhib products_exhib_exhib id",
                        column: x => x.exhibid,
                        principalTable: "exhib",
                        principalColumn: "exhib id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hours",
                columns: table => new
                {
                    HourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<TimeSpan>(type: "time", nullable: false),
                    taskid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hours", x => x.HourId);
                });

            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    identityid = table.Column<int>(name: "identity id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    invent_id = table.Column<int>(type: "int", nullable: true),
                    exhib_id = table.Column<int>(type: "int", nullable: true),
                    RemovePro_id = table.Column<int>(type: "int", nullable: true),
                    Addpro_id = table.Column<int>(type: "int", nullable: true),
                    Replacepro_id = table.Column<int>(type: "int", nullable: true),
                    serialnumber = table.Column<string>(name: "serial number", type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    removestatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deliverystatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unitprice = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.identityid);
                    table.ForeignKey(
                        name: "FK_Identity_Add product_Addpro_id",
                        column: x => x.Addpro_id,
                        principalTable: "Add product",
                        principalColumn: "AddPro id");
                    table.ForeignKey(
                        name: "FK_Identity_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Identity_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Identity_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Identity_exhib_exhib_id",
                        column: x => x.exhib_id,
                        principalTable: "exhib",
                        principalColumn: "exhib id");
                });

            migrationBuilder.CreateTable(
                name: "identity_products",
                columns: table => new
                {
                    identityid = table.Column<int>(name: "identity id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_products", x => x.identityid);
                    table.ForeignKey(
                        name: "FK_identity_products_Identity_identity id",
                        column: x => x.identityid,
                        principalTable: "Identity",
                        principalColumn: "identity id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "instructions",
                columns: table => new
                {
                    InstructionID = table.Column<int>(name: "Instruction ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructionName = table.Column<string>(name: "Instruction Name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructions", x => x.InstructionID);
                });

            migrationBuilder.CreateTable(
                name: "invent",
                columns: table => new
                {
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invent_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: true),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<double>(type: "float", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invent", x => x.inventid);
                    table.ForeignKey(
                        name: "FK_invent_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_invent_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_invent_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_invent_address_address_id",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "address id");
                });

            migrationBuilder.CreateTable(
                name: "invent products",
                columns: table => new
                {
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    previousunit = table.Column<double>(type: "float", nullable: true),
                    finalunit = table.Column<double>(type: "float", nullable: true),
                    previouscartoon = table.Column<double>(type: "float", nullable: true),
                    finalcartoon = table.Column<double>(type: "float", nullable: true),
                    previousroll = table.Column<double>(type: "float", nullable: true),
                    finalroll = table.Column<double>(type: "float", nullable: true),
                    goodunit = table.Column<double>(type: "float", nullable: true),
                    goodcartoon = table.Column<double>(type: "float", nullable: true),
                    goodroll = table.Column<double>(type: "float", nullable: true),
                    badunit = table.Column<double>(type: "float", nullable: true),
                    badcartoon = table.Column<double>(type: "float", nullable: true),
                    badroll = table.Column<double>(type: "float", nullable: true),
                    usedunit = table.Column<double>(type: "float", nullable: true),
                    usedcartoon = table.Column<double>(type: "float", nullable: true),
                    usedroll = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invent products", x => x.inventid);
                    table.ForeignKey(
                        name: "FK_invent products_invent_invent id",
                        column: x => x.inventid,
                        principalTable: "invent",
                        principalColumn: "invent id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Milestone",
                columns: table => new
                {
                    Milestoneid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MilestoneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Planning_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Planning_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    statusid = table.Column<int>(type: "int", nullable: true),
                    objectiveid = table.Column<int>(type: "int", nullable: true),
                    Project_projectid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestone", x => x.Milestoneid);
                    table.ForeignKey(
                        name: "FK_Milestone_Status_statusid",
                        column: x => x.statusid,
                        principalTable: "Status",
                        principalColumn: "Statusid");
                });

            migrationBuilder.CreateTable(
                name: "miss_products",
                columns: table => new
                {
                    missid = table.Column<int>(name: "miss id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_miss_products", x => x.missid);
                });

            migrationBuilder.CreateTable(
                name: "MissingItem",
                columns: table => new
                {
                    missid = table.Column<int>(name: "miss id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Remove_id = table.Column<int>(type: "int", nullable: true),
                    RemovePro_id = table.Column<int>(type: "int", nullable: true),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissingItem", x => x.missid);
                    table.ForeignKey(
                        name: "FK_MissingItem_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_MissingItem_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_MissingItem_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Notifyid = table.Column<int>(name: "Notify id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "text", nullable: false),
                    nameofoperation = table.Column<string>(name: "name of operation", type: "text", nullable: false),
                    affectedobject = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Receiver = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<int>(type: "int", nullable: true),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Notifyid);
                    table.ForeignKey(
                        name: "FK_Notification_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kind = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MetaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objective",
                columns: table => new
                {
                    objectiveid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tickid = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objective", x => x.objectiveid);
                });

            migrationBuilder.CreateTable(
                name: "Obstackle",
                columns: table => new
                {
                    ObsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Obs_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Obs_Descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObsDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    solu_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    solu_Descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    soluDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Taskid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obstackle", x => x.ObsId);
                });

            migrationBuilder.CreateTable(
                name: "PersonalInformation",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    nameofoperation = table.Column<string>(name: "name of operation", type: "text", nullable: true),
                    affectedobject = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Receiver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    First_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trial = table.Column<int>(type: "int", nullable: true),
                    Accounting = table.Column<int>(type: "int", nullable: true),
                    Finincial = table.Column<int>(type: "int", nullable: true),
                    System = table.Column<int>(type: "int", nullable: true),
                    Chat = table.Column<int>(type: "int", nullable: true),
                    Event = table.Column<int>(type: "int", nullable: true),
                    Graphs = table.Column<int>(type: "int", nullable: true),
                    Social = table.Column<int>(type: "int", nullable: true),
                    Project = table.Column<int>(type: "int", nullable: true),
                    CProject = table.Column<int>(type: "int", nullable: true),
                    Notification = table.Column<int>(type: "int", nullable: true),
                    Todo = table.Column<int>(type: "int", nullable: true),
                    Complain = table.Column<int>(type: "int", nullable: true),
                    TT = table.Column<int>(type: "int", nullable: true),
                    Last_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Read = table.Column<bool>(type: "bit", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EXdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: true),
                    Extension = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInformation", x => x.person_id);
                    table.ForeignKey(
                        name: "FK_PersonalInformation_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<int>(name: "Post ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderid = table.Column<int>(type: "int", nullable: true),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);
                    table.ForeignKey(
                        name: "FK_Post_Employee_senderid",
                        column: x => x.senderid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Post_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Post_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    projectid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    projectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Planning_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Planning_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hourly_Rate = table.Column<double>(type: "float", nullable: false),
                    budget = table.Column<double>(type: "float", nullable: false),
                    Active = table.Column<int>(type: "int", nullable: false),
                    TOtalDayes = table.Column<double>(type: "float", nullable: false),
                    Labor_cost = table.Column<double>(type: "float", nullable: false),
                    MaterialCosts = table.Column<double>(type: "float", nullable: false),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    TotalCost = table.Column<double>(type: "float", nullable: false),
                    companyid = table.Column<int>(type: "int", nullable: true),
                    statusid = table.Column<int>(type: "int", nullable: true),
                    teamid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.projectid);
                    table.ForeignKey(
                        name: "FK_Project_Company_companyid",
                        column: x => x.companyid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Project_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Project_Status_statusid",
                        column: x => x.statusid,
                        principalTable: "Status",
                        principalColumn: "Statusid");
                    table.ForeignKey(
                        name: "FK_Project_Team_teamid",
                        column: x => x.teamid,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "Purchases Replace",
                columns: table => new
                {
                    Replaceid = table.Column<int>(name: "Replace id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    discount = table.Column<double>(type: "float", nullable: true),
                    code = table.Column<int>(type: "int", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    subtotal = table.Column<double>(type: "float", nullable: true),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases Replace", x => x.Replaceid);
                    table.ForeignKey(
                        name: "FK_Purchases Replace_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Purchases Replace_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Purchases Replace_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Purchases Replace_Importer_importer id",
                        column: x => x.importerid,
                        principalTable: "Importer",
                        principalColumn: "importer id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases Replace_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Purchses Bill",
                columns: table => new
                {
                    Billid = table.Column<int>(name: "Bill id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: false),
                    previousbalance = table.Column<double>(type: "float", nullable: false),
                    finalbalance = table.Column<double>(type: "float", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    code = table.Column<int>(type: "int", nullable: true),
                    discount = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: false),
                    subtotal = table.Column<double>(type: "float", nullable: false),
                    Descrip = table.Column<string>(type: "text", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchses Bill", x => x.Billid);
                    table.ForeignKey(
                        name: "FK_Purchses Bill_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Purchses Bill_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Purchses Bill_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Purchses Bill_Importer_importer id",
                        column: x => x.importerid,
                        principalTable: "Importer",
                        principalColumn: "importer id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchses Bill_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Remove Receipt",
                columns: table => new
                {
                    Removeid = table.Column<int>(name: "Remove id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: true),
                    billnumber = table.Column<int>(name: "bill number", type: "int", nullable: true),
                    code = table.Column<int>(type: "int", nullable: true),
                    Deliveredto = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: true),
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<double>(type: "float", nullable: true),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true),
                    CustomerCustomter_id = table.Column<int>(type: "int", nullable: true),
                    Sales_BillBill_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remove Receipt", x => x.Removeid);
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Customer_CustomerCustomter_id",
                        column: x => x.CustomerCustomter_id,
                        principalTable: "Customer",
                        principalColumn: "Customer id");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Importer_importer id",
                        column: x => x.importerid,
                        principalTable: "Importer",
                        principalColumn: "importer id");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_Sales Bill_Sales_BillBill_id",
                        column: x => x.Sales_BillBill_id,
                        principalTable: "Sales Bill",
                        principalColumn: "Bill id");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_exhib_exhib id",
                        column: x => x.exhibid,
                        principalTable: "exhib",
                        principalColumn: "exhib id");
                    table.ForeignKey(
                        name: "FK_Remove Receipt_invent_invent id",
                        column: x => x.inventid,
                        principalTable: "invent",
                        principalColumn: "invent id");
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestID = table.Column<int>(name: "Request ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestName = table.Column<string>(name: "Request Name", type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employerid = table.Column<int>(type: "int", nullable: true),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    Depid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Request_Department_Depid",
                        column: x => x.Depid,
                        principalTable: "Department",
                        principalColumn: "Department ID");
                    table.ForeignKey(
                        name: "FK_Request_Employee_employerid",
                        column: x => x.employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Request_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Request_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Stock Taking",
                columns: table => new
                {
                    Stocktakeid = table.Column<int>(name: "Stocktake id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock Taking", x => x.Stocktakeid);
                    table.ForeignKey(
                        name: "FK_Stock Taking_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Stock Taking_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Stock Taking_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Stock Taking_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "ToDo",
                columns: table => new
                {
                    ToDoID = table.Column<int>(name: "ToDo ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    importance = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lasttransaction = table.Column<int>(type: "int", nullable: false),
                    endtime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senderid = table.Column<int>(type: "int", nullable: true),
                    solverid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDo", x => x.ToDoID);
                    table.ForeignKey(
                        name: "FK_ToDo_Employee_senderid",
                        column: x => x.senderid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_ToDo_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_ToDo_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Unity",
                columns: table => new
                {
                    unityid = table.Column<int>(name: "unity id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cartoon = table.Column<string>(type: "text", nullable: false),
                    Roll = table.Column<string>(type: "text", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unity", x => x.unityid);
                    table.ForeignKey(
                        name: "FK_Unity_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Unity_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Unity_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Unity_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    employeeid = table.Column<int>(type: "int", nullable: true),
                    profileid = table.Column<int>(type: "int", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdObjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employee_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Users_PersonalInformation_profileid",
                        column: x => x.profileid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "RequestResponse",
                columns: table => new
                {
                    RequestResID = table.Column<int>(name: "RequestRes ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employerid = table.Column<int>(type: "int", nullable: true),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    RequestResName = table.Column<string>(name: "RequestRes Name", type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Depid = table.Column<int>(type: "int", nullable: true),
                    Reqid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestResponse", x => x.RequestResID);
                    table.ForeignKey(
                        name: "FK_RequestResponse_Department_Depid",
                        column: x => x.Depid,
                        principalTable: "Department",
                        principalColumn: "Department ID");
                    table.ForeignKey(
                        name: "FK_RequestResponse_Employee_employerid",
                        column: x => x.employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_RequestResponse_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_RequestResponse_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_RequestResponse_Request_Reqid",
                        column: x => x.Reqid,
                        principalTable: "Request",
                        principalColumn: "Request ID");
                });

            migrationBuilder.CreateTable(
                name: "TroubleTicket",
                columns: table => new
                {
                    Ticketid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StopTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    issuerUserID = table.Column<int>(type: "int", nullable: true),
                    Period = table.Column<TimeSpan>(type: "time", nullable: false),
                    statusid = table.Column<int>(type: "int", nullable: true),
                    depid = table.Column<int>(type: "int", nullable: true),
                    personalid = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    typid = table.Column<int>(type: "int", nullable: true),
                    syscomid = table.Column<int>(type: "int", nullable: true),
                    catid = table.Column<long>(type: "bigint", nullable: true),
                    Reqid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TroubleTicket", x => x.Ticketid);
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Category_catid",
                        column: x => x.catid,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Department_depid",
                        column: x => x.depid,
                        principalTable: "Department",
                        principalColumn: "Department ID");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Employee_issuerUserID",
                        column: x => x.issuerUserID,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Group_groupid",
                        column: x => x.groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_PersonalInformation_personalid",
                        column: x => x.personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Request_Reqid",
                        column: x => x.Reqid,
                        principalTable: "Request",
                        principalColumn: "Request ID");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Status_statusid",
                        column: x => x.statusid,
                        principalTable: "Status",
                        principalColumn: "Statusid");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_SystemComponent_syscomid",
                        column: x => x.syscomid,
                        principalTable: "SystemComponent",
                        principalColumn: "SyscomId");
                    table.ForeignKey(
                        name: "FK_TroubleTicket_Type_typid",
                        column: x => x.typid,
                        principalTable: "Type",
                        principalColumn: "Type ID");
                });

            migrationBuilder.CreateTable(
                name: "Transaction2",
                columns: table => new
                {
                    Transid = table.Column<int>(name: "Trans id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stocktakeid = table.Column<int>(name: "stocktake id", type: "int", nullable: true),
                    nameofoperation = table.Column<string>(name: "name of operation", type: "text", nullable: false),
                    affectedobject = table.Column<string>(type: "text", nullable: false),
                    Previous = table.Column<string>(type: "text", nullable: false),
                    Current = table.Column<string>(type: "text", nullable: false),
                    Change = table.Column<string>(type: "text", nullable: false),
                    idOO = table.Column<int>(type: "int", nullable: false),
                    NOO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction2", x => x.Transid);
                    table.ForeignKey(
                        name: "FK_Transaction2_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Transaction2_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Transaction2_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Transaction2_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Transaction2_Stock Taking_stocktake id",
                        column: x => x.stocktakeid,
                        principalTable: "Stock Taking",
                        principalColumn: "Stocktake id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(name: "Product ID", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(name: "product Name", type: "text", nullable: false),
                    Descrition = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    length = table.Column<double>(type: "float", nullable: true),
                    width = table.Column<double>(type: "float", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    buyprice = table.Column<double>(name: "buy price", type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    cartoon = table.Column<double>(type: "float", nullable: true),
                    Roll = table.Column<double>(type: "float", nullable: true),
                    unit = table.Column<double>(type: "float", nullable: true),
                    productcode = table.Column<int>(name: "product code", type: "int", nullable: true),
                    importerid = table.Column<int>(name: "importer id", type: "int", nullable: true),
                    unityid = table.Column<int>(name: "unity id", type: "int", nullable: true),
                    goodunit = table.Column<double>(type: "float", nullable: true),
                    badunit = table.Column<double>(type: "float", nullable: true),
                    usedunit = table.Column<double>(type: "float", nullable: true),
                    goodroll = table.Column<double>(type: "float", nullable: true),
                    badroll = table.Column<double>(type: "float", nullable: true),
                    usedroll = table.Column<double>(type: "float", nullable: true),
                    Saleprice = table.Column<double>(type: "float", nullable: true),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true),
                    CustomerCustomter_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Product_Customer_CustomerCustomter_id",
                        column: x => x.CustomerCustomter_id,
                        principalTable: "Customer",
                        principalColumn: "Customer id");
                    table.ForeignKey(
                        name: "FK_Product_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Product_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Product_Importer_importer id",
                        column: x => x.importerid,
                        principalTable: "Importer",
                        principalColumn: "importer id");
                    table.ForeignKey(
                        name: "FK_Product_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Product_Unity_unity id",
                        column: x => x.unityid,
                        principalTable: "Unity",
                        principalColumn: "unity id");
                });

            migrationBuilder.CreateTable(
                name: "UserLog",
                columns: table => new
                {
                    UserLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => x.UserLogID);
                    table.ForeignKey(
                        name: "FK_UserLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Taskid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tickid = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    resolved = table.Column<bool>(type: "bit", nullable: false),
                    Planning_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Planning_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actual_StopDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employeeid = table.Column<int>(type: "int", nullable: true),
                    milestoneid = table.Column<int>(type: "int", nullable: true),
                    Reqid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Taskid);
                    table.ForeignKey(
                        name: "FK_Task_Employee_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Task_Milestone_milestoneid",
                        column: x => x.milestoneid,
                        principalTable: "Milestone",
                        principalColumn: "Milestoneid");
                    table.ForeignKey(
                        name: "FK_Task_Request_Reqid",
                        column: x => x.Reqid,
                        principalTable: "Request",
                        principalColumn: "Request ID");
                    table.ForeignKey(
                        name: "FK_Task_TroubleTicket_Tickid",
                        column: x => x.Tickid,
                        principalTable: "TroubleTicket",
                        principalColumn: "Ticketid");
                });

            migrationBuilder.CreateTable(
                name: "Remove Product",
                columns: table => new
                {
                    removeid = table.Column<int>(name: "remove id", type: "int", nullable: false),
                    productid = table.Column<int>(name: "product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    Delivery_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Delivery_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    invent_id = table.Column<int>(type: "int", nullable: true),
                    exhib_id = table.Column<int>(type: "int", nullable: true),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemoveProid = table.Column<int>(name: " RemovePro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remove Product", x => x.RemoveProid);
                    table.ForeignKey(
                        name: "FK_Remove Product_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Remove Product_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Remove Product_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Remove Product_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Remove Product_Product_product id",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "Product ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Remove Product_Remove Receipt_remove id",
                        column: x => x.removeid,
                        principalTable: "Remove Receipt",
                        principalColumn: "Remove id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Remove Product_exhib_exhib_id",
                        column: x => x.exhib_id,
                        principalTable: "exhib",
                        principalColumn: "exhib id");
                    table.ForeignKey(
                        name: "FK_Remove Product_invent_invent_id",
                        column: x => x.invent_id,
                        principalTable: "invent",
                        principalColumn: "invent id");
                });

            migrationBuilder.CreateTable(
                name: "Replace Products",
                columns: table => new
                {
                    replaceid = table.Column<int>(name: "replace id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    ReplaceProid = table.Column<int>(name: "ReplacePro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: true),
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unitprice = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employerid = table.Column<int>(type: "int", nullable: true),
                    Personalid = table.Column<int>(type: "int", nullable: true),
                    Compid = table.Column<int>(type: "int", nullable: true),
                    Groupid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replace Products", x => x.ReplaceProid);
                    table.ForeignKey(
                        name: "FK_Replace Products_Company_Compid",
                        column: x => x.Compid,
                        principalTable: "Company",
                        principalColumn: "companyid");
                    table.ForeignKey(
                        name: "FK_Replace Products_Employee_Employerid",
                        column: x => x.Employerid,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_Replace Products_Group_Groupid",
                        column: x => x.Groupid,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Replace Products_PersonalInformation_Personalid",
                        column: x => x.Personalid,
                        principalTable: "PersonalInformation",
                        principalColumn: "person_id");
                    table.ForeignKey(
                        name: "FK_Replace Products_Product_Product id",
                        column: x => x.Productid,
                        principalTable: "Product",
                        principalColumn: "Product ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Replace Products_Purchases Replace_replace id",
                        column: x => x.replaceid,
                        principalTable: "Purchases Replace",
                        principalColumn: "Replace id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Replace Products_exhib_exhib id",
                        column: x => x.exhibid,
                        principalTable: "exhib",
                        principalColumn: "exhib id");
                    table.ForeignKey(
                        name: "FK_Replace Products_invent_invent id",
                        column: x => x.inventid,
                        principalTable: "invent",
                        principalColumn: "invent id");
                });

            migrationBuilder.CreateTable(
                name: "SalesBill Products",
                columns: table => new
                {
                    BillProid = table.Column<int>(name: "BillPro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Billid = table.Column<int>(name: "Bill id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addproid = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unitprice = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    descrip = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesBill Products", x => x.BillProid);
                    table.ForeignKey(
                        name: "FK_SalesBill Products_Product_Product id",
                        column: x => x.Productid,
                        principalTable: "Product",
                        principalColumn: "Product ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesBill Products_Sales Bill_Bill id",
                        column: x => x.Billid,
                        principalTable: "Sales Bill",
                        principalColumn: "Bill id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesReplace Products",
                columns: table => new
                {
                    replaceid = table.Column<int>(name: "replace id", type: "int", nullable: false),
                    Productid = table.Column<int>(name: "Product id", type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    ReplaceProid = table.Column<int>(name: "ReplacePro id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inventid = table.Column<int>(name: "invent id", type: "int", nullable: true),
                    exhibid = table.Column<int>(name: "exhib id", type: "int", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unitprice = table.Column<double>(type: "float", nullable: true),
                    total = table.Column<double>(type: "float", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReplace Products", x => x.ReplaceProid);
                    table.ForeignKey(
                        name: "FK_SalesReplace Products_Product_Product id",
                        column: x => x.Productid,
                        principalTable: "Product",
                        principalColumn: "Product ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesReplace Products_Sales Replace_replace id",
                        column: x => x.replaceid,
                        principalTable: "Sales Replace",
                        principalColumn: "Replace id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesReplace Products_exhib_exhib id",
                        column: x => x.exhibid,
                        principalTable: "exhib",
                        principalColumn: "exhib id");
                    table.ForeignKey(
                        name: "FK_SalesReplace Products_invent_invent id",
                        column: x => x.inventid,
                        principalTable: "invent",
                        principalColumn: "invent id");
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    TeamMemberid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamMemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    teamid = table.Column<int>(type: "int", nullable: true),
                    employee_id = table.Column<int>(type: "int", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    Tasks_Taskid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.TeamMemberid);
                    table.ForeignKey(
                        name: "FK_TeamMember_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "Employer ID");
                    table.ForeignKey(
                        name: "FK_TeamMember_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                    table.ForeignKey(
                        name: "FK_TeamMember_Task_Tasks_Taskid",
                        column: x => x.Tasks_Taskid,
                        principalTable: "Task",
                        principalColumn: "Taskid");
                    table.ForeignKey(
                        name: "FK_TeamMember_Team_teamid",
                        column: x => x.teamid,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.InsertData(
                table: "CharityAidTypes",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), 1, true, "نقدي", "Cash" },
                    { new Guid("55555555-5555-5555-5555-555555555552"), 2, true, "غذائي", "Food" },
                    { new Guid("55555555-5555-5555-5555-555555555553"), 3, true, "علاجي", "Medical" },
                    { new Guid("55555555-5555-5555-5555-555555555554"), 4, true, "تعليمي", "Educational" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 5, true, "ملابس", "Clothes" },
                    { new Guid("55555555-5555-5555-5555-555555555556"), 6, true, "أجهزة", "Devices" },
                    { new Guid("55555555-5555-5555-5555-555555555557"), 7, true, "كفالة", "Sponsorship" }
                });

            migrationBuilder.InsertData(
                table: "CharityBeneficiaryStatuses",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), 1, true, "جديد", "New" },
                    { new Guid("44444444-4444-4444-4444-444444444442"), 2, true, "تحت الدراسة", "Under Review" },
                    { new Guid("44444444-4444-4444-4444-444444444443"), 3, true, "معتمد", "Approved" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 4, true, "موقوف", "Suspended" },
                    { new Guid("44444444-4444-4444-4444-444444444445"), 5, true, "مرفوض", "Rejected" }
                });

            migrationBuilder.InsertData(
                table: "CharityGenders",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1, true, "ذكر", "Male" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 2, true, "أنثى", "Female" }
                });

            migrationBuilder.InsertData(
                table: "CharityMaritalStatuses",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), 1, true, "أعزب", "Single" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), 2, true, "متزوج", "Married" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 3, true, "أرمل", "Widowed" },
                    { new Guid("33333333-3333-3333-3333-333333333334"), 4, true, "مطلق", "Divorced" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Employeeid",
                table: "Account",
                column: "Employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Account_TreasuryId",
                table: "Account",
                column: "TreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCostCenters_CostCenterCode",
                table: "AccountingCostCenters",
                column: "CostCenterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCostCenters_ParentCostCenterId",
                table: "AccountingCostCenters",
                column: "ParentCostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingFiscalPeriods_PeriodCode",
                table: "AccountingFiscalPeriods",
                column: "PeriodCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_CreditAccountId",
                table: "AccountingIntegrationProfiles",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_DebitAccountId",
                table: "AccountingIntegrationProfiles",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_DefaultCostCenterId",
                table: "AccountingIntegrationProfiles",
                column: "DefaultCostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_SourceType",
                table: "AccountingIntegrationProfiles",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_SourceType_EventCode_IsActive",
                table: "AccountingIntegrationProfiles",
                columns: new[] { "SourceType", "EventCode", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationProfiles_SourceType_MatchDonationType_MatchTargetingScopeCode",
                table: "AccountingIntegrationProfiles",
                columns: new[] { "SourceType", "MatchDonationType", "MatchTargetingScopeCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationSourceDefinitions_IsActive",
                table: "AccountingIntegrationSourceDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationSourceDefinitions_SourceType",
                table: "AccountingIntegrationSourceDefinitions",
                column: "SourceType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingIntegrationSourceDefinitions_SourceType_IsDynamicPostingEnabled",
                table: "AccountingIntegrationSourceDefinitions",
                columns: new[] { "SourceType", "IsDynamicPostingEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournalEntries_EntryNumber",
                table: "AccountingJournalEntries",
                column: "EntryNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournalEntries_FiscalPeriodId",
                table: "AccountingJournalEntries",
                column: "FiscalPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournalEntryLines_CostCenterId",
                table: "AccountingJournalEntryLines",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournalEntryLines_FinancialAccountId",
                table: "AccountingJournalEntryLines",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournalEntryLines_JournalEntryId",
                table: "AccountingJournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingProfiles_Code",
                table: "AccountingPostingProfiles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingProfiles_CreditAccountId",
                table: "AccountingPostingProfiles",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingProfiles_DebitAccountId",
                table: "AccountingPostingProfiles",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingProfiles_DefaultCostCenterId",
                table: "AccountingPostingProfiles",
                column: "DefaultCostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingProfiles_EventCode_TargetingScopeCode_PurposeName_DonationType",
                table: "AccountingPostingProfiles",
                columns: new[] { "EventCode", "TargetingScopeCode", "PurposeName", "DonationType" });

            migrationBuilder.CreateIndex(
                name: "IX_Add product_add id",
                table: "Add product",
                column: "add id");

            migrationBuilder.CreateIndex(
                name: "IX_Add product_Compid",
                table: "Add product",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Add product_Employerid",
                table: "Add product",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Add product_Groupid",
                table: "Add product",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Add product_Personalid",
                table: "Add product",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Add product_Product id",
                table: "Add product",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_bill id",
                table: "Add Receipt",
                column: "bill id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_Compid",
                table: "Add Receipt",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_CustomerCustomter_id",
                table: "Add Receipt",
                column: "CustomerCustomter_id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_Employerid",
                table: "Add Receipt",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_exhib id",
                table: "Add Receipt",
                column: "exhib id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_Groupid",
                table: "Add Receipt",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_importer id",
                table: "Add Receipt",
                column: "importer id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_invent id",
                table: "Add Receipt",
                column: "invent id");

            migrationBuilder.CreateIndex(
                name: "IX_Add Receipt_Personalid",
                table: "Add Receipt",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_address_Compid",
                table: "address",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_address_Employerid",
                table: "address",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_address_Groupid",
                table: "address",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_address_Personalid",
                table: "address",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryContactLogs_BeneficiaryId",
                table: "BeneficiaryContactLogs",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryContactLogs_FollowUpDate",
                table: "BeneficiaryContactLogs",
                column: "FollowUpDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Bill id",
                table: "Bill Products",
                column: "Bill id");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Compid",
                table: "Bill Products",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Employerid",
                table: "Bill Products",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Groupid",
                table: "Bill Products",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Personalid",
                table: "Bill Products",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Bill Products_Product id",
                table: "Bill Products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_boardUsers_UserId_BoardId",
                table: "boardUsers",
                columns: new[] { "UserId", "BoardId" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarAttendees_EventId_UserId",
                table: "CalendarAttendees",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarAttendees_UserId",
                table: "CalendarAttendees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CreatedByUserId",
                table: "CalendarEvents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_StartUtc_EndUtc",
                table: "CalendarEvents",
                columns: new[] { "StartUtc", "EndUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Category_balanceID",
                table: "Category",
                column: "balanceID");

            migrationBuilder.CreateIndex(
                name: "IX_Category_LimitID",
                table: "Category",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_CharityActivityPhaseAssignments_ActivityId",
                table: "CharityActivityPhaseAssignments",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityActivityPhaseAssignments_PhaseId",
                table: "CharityActivityPhaseAssignments",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_AidCycleId_BeneficiaryId",
                table: "CharityAidCycleBeneficiaries",
                columns: new[] { "AidCycleId", "BeneficiaryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_AidTypeId",
                table: "CharityAidCycleBeneficiaries",
                column: "AidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_BeneficiaryId",
                table: "CharityAidCycleBeneficiaries",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_CommitteeDecisionId",
                table: "CharityAidCycleBeneficiaries",
                column: "CommitteeDecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_NextDueDate",
                table: "CharityAidCycleBeneficiaries",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycleBeneficiaries_Status",
                table: "CharityAidCycleBeneficiaries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycles_AidTypeId",
                table: "CharityAidCycles",
                column: "AidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycles_ApprovedByUserId",
                table: "CharityAidCycles",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycles_CreatedByUserId",
                table: "CharityAidCycles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycles_CycleNumber",
                table: "CharityAidCycles",
                column: "CycleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidCycles_PeriodYear_PeriodMonth_CycleType",
                table: "CharityAidCycles",
                columns: new[] { "PeriodYear", "PeriodMonth", "CycleType" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursementFundingLines_CreatedByUserId",
                table: "CharityAidDisbursementFundingLines",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursementFundingLines_DisbursementId",
                table: "CharityAidDisbursementFundingLines",
                column: "DisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursementFundingLines_DonationAllocationId",
                table: "CharityAidDisbursementFundingLines",
                column: "DonationAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_AidRequestId",
                table: "CharityAidDisbursements",
                column: "AidRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_AidTypeId",
                table: "CharityAidDisbursements",
                column: "AidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_ApprovalStatus",
                table: "CharityAidDisbursements",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_ApprovedByUserId",
                table: "CharityAidDisbursements",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_BeneficiaryId",
                table: "CharityAidDisbursements",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_CreatedByUserId",
                table: "CharityAidDisbursements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_ExecutedByUserId",
                table: "CharityAidDisbursements",
                column: "ExecutedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_ExecutionStatus",
                table: "CharityAidDisbursements",
                column: "ExecutionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_FinancialAccountId",
                table: "CharityAidDisbursements",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_PaymentMethodId",
                table: "CharityAidDisbursements",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_RejectedByUserId",
                table: "CharityAidDisbursements",
                column: "RejectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_SourceType_SourceId",
                table: "CharityAidDisbursements",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidRequests_AidTypeId",
                table: "CharityAidRequests",
                column: "AidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidRequests_BeneficiaryId",
                table: "CharityAidRequests",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidRequests_CreatedByUserId",
                table: "CharityAidRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidRequests_ProjectId",
                table: "CharityAidRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAreas_CityId_NameAr",
                table: "CharityAreas",
                columns: new[] { "CityId", "NameAr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_AreaId",
                table: "CharityBeneficiaries",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_CityId",
                table: "CharityBeneficiaries",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_Code",
                table: "CharityBeneficiaries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_CreatedByUserId",
                table: "CharityBeneficiaries",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_GenderId",
                table: "CharityBeneficiaries",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_GovernorateId",
                table: "CharityBeneficiaries",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_MaritalStatusId",
                table: "CharityBeneficiaries",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_NationalId",
                table: "CharityBeneficiaries",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_PhoneNumber",
                table: "CharityBeneficiaries",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaries_StatusId",
                table: "CharityBeneficiaries",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryAssessments_BeneficiaryId",
                table: "CharityBeneficiaryAssessments",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryAssessments_RecommendedAidTypeId",
                table: "CharityBeneficiaryAssessments",
                column: "RecommendedAidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryAssessments_ResearcherUserId",
                table: "CharityBeneficiaryAssessments",
                column: "ResearcherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCommitteeDecisions_ApprovedAidTypeId",
                table: "CharityBeneficiaryCommitteeDecisions",
                column: "ApprovedAidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCommitteeDecisions_ApprovedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCommitteeDecisions_BeneficiaryId",
                table: "CharityBeneficiaryCommitteeDecisions",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryDocuments_BeneficiaryId",
                table: "CharityBeneficiaryDocuments",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryFamilyMembers_BeneficiaryId",
                table: "CharityBeneficiaryFamilyMembers",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryFamilyMembers_GenderId",
                table: "CharityBeneficiaryFamilyMembers",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchCommitteeEvaluations_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchCommitteeEvaluations",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchDebts_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchDebts",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchExpenseItems_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchExpenseItems",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchFamilyMembers_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchFamilyMembers",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchHouseAssets_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchHouseAssets",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchIncomeItems_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchIncomeItems",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchReviews_ResearchId",
                table: "CharityBeneficiaryHumanitarianResearchReviews",
                column: "ResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchs_BeneficiaryId",
                table: "CharityBeneficiaryHumanitarianResearchs",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryHumanitarianResearchs_ResearchNumber",
                table: "CharityBeneficiaryHumanitarianResearchs",
                column: "ResearchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryOldRecords_BeneficiaryId",
                table: "CharityBeneficiaryOldRecords",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryOldRecords_CreatedByUserId",
                table: "CharityBeneficiaryOldRecords",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardDecisionAttachments_BoardDecisionId",
                table: "CharityBoardDecisionAttachments",
                column: "BoardDecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardDecisionFollowUps_BoardDecisionId",
                table: "CharityBoardDecisionFollowUps",
                column: "BoardDecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardDecisions_BoardMeetingId",
                table: "CharityBoardDecisions",
                column: "BoardMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardDecisions_DecisionNumber",
                table: "CharityBoardDecisions",
                column: "DecisionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardMeetingAttachments_BoardMeetingId",
                table: "CharityBoardMeetingAttachments",
                column: "BoardMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardMeetingAttendees_BoardMeetingId",
                table: "CharityBoardMeetingAttendees",
                column: "BoardMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardMeetingMinutes_BoardMeetingId",
                table: "CharityBoardMeetingMinutes",
                column: "BoardMeetingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityBoardMeetings_MeetingNumber",
                table: "CharityBoardMeetings",
                column: "MeetingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityCities_GovernorateId_NameAr",
                table: "CharityCities",
                columns: new[] { "GovernorateId", "NameAr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_AidRequestId",
                table: "CharityDonationAllocations",
                column: "AidRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_AllocatedDate",
                table: "CharityDonationAllocations",
                column: "AllocatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_ApprovalStatus",
                table: "CharityDonationAllocations",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_BeneficiaryId",
                table: "CharityDonationAllocations",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_DonationId",
                table: "CharityDonationAllocations",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_DonationInKindItemId",
                table: "CharityDonationAllocations",
                column: "DonationInKindItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationInKindItems_DonationId",
                table: "CharityDonationInKindItems",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationInKindItems_ItemId",
                table: "CharityDonationInKindItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationInKindItems_WarehouseId",
                table: "CharityDonationInKindItems",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_AidTypeId",
                table: "CharityDonations",
                column: "AidTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_CreatedByUserId",
                table: "CharityDonations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_DonationDate",
                table: "CharityDonations",
                column: "DonationDate");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_DonationNumber",
                table: "CharityDonations",
                column: "DonationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_DonorId",
                table: "CharityDonations",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_FinancialAccountId",
                table: "CharityDonations",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_PaymentMethodId",
                table: "CharityDonations",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_ReceiptNumber",
                table: "CharityDonations",
                column: "ReceiptNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonations_TargetingScopeCode",
                table: "CharityDonations",
                column: "TargetingScopeCode");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_AreaId",
                table: "CharityDonors",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_CityId",
                table: "CharityDonors",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_Code",
                table: "CharityDonors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_GovernorateId",
                table: "CharityDonors",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_NationalIdOrTaxNo",
                table: "CharityDonors",
                column: "NationalIdOrTaxNo",
                unique: true,
                filter: "[NationalIdOrTaxNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonors_PhoneNumber",
                table: "CharityDonors",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CharityEmployeeSalaryStructures_EmployeeId",
                table: "CharityEmployeeSalaryStructures",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityEmployeeSalaryStructures_SalaryItemDefinitionId",
                table: "CharityEmployeeSalaryStructures",
                column: "SalaryItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityFunders_Code",
                table: "CharityFunders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityFunders_FunderType_IsActive",
                table: "CharityFunders",
                columns: new[] { "FunderType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityFunders_Name",
                table: "CharityFunders",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CharityGovernorates_NameAr",
                table: "CharityGovernorates",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantAgreements_AgreementNumber",
                table: "CharityGrantAgreements",
                column: "AgreementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantAgreements_FunderId_Status",
                table: "CharityGrantAgreements",
                columns: new[] { "FunderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantConditions_GrantAgreementId_IsFulfilled_IsMandatory",
                table: "CharityGrantConditions",
                columns: new[] { "GrantAgreementId", "IsFulfilled", "IsMandatory" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantInstallments_DueDate_Status",
                table: "CharityGrantInstallments",
                columns: new[] { "DueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantInstallments_FinancialAccountId",
                table: "CharityGrantInstallments",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantInstallments_GrantAgreementId_InstallmentNumber",
                table: "CharityGrantInstallments",
                columns: new[] { "GrantAgreementId", "InstallmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityGrantInstallments_PaymentMethodId",
                table: "CharityGrantInstallments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrAttendanceRecords_EmployeeId_AttendanceDate",
                table: "CharityHrAttendanceRecords",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrAttendanceRecords_ShiftId",
                table: "CharityHrAttendanceRecords",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrDepartments_Name",
                table: "CharityHrDepartments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployeeBonuses_EmployeeId",
                table: "CharityHrEmployeeBonuses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployeeContracts_EmployeeId",
                table: "CharityHrEmployeeContracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployeeFundingAssignments_EmployeeId",
                table: "CharityHrEmployeeFundingAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployees_Code",
                table: "CharityHrEmployees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployees_DepartmentId",
                table: "CharityHrEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployees_JobTitleId",
                table: "CharityHrEmployees",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployees_NationalId",
                table: "CharityHrEmployees",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployees_PhoneNumber",
                table: "CharityHrEmployees",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrEmployeeTaskAssignments_EmployeeId",
                table: "CharityHrEmployeeTaskAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrJobTitles_Name",
                table: "CharityHrJobTitles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityHrShifts_Name",
                table: "CharityHrShifts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaCases_BeneficiaryId",
                table: "CharityKafalaCases",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaCases_CaseNumber",
                table: "CharityKafalaCases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaCases_FinancialAccountId",
                table: "CharityKafalaCases",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaCases_PaymentMethodId",
                table: "CharityKafalaCases",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaCases_SponsorId",
                table: "CharityKafalaCases",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaPayments_FinancialAccountId",
                table: "CharityKafalaPayments",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaPayments_KafalaCaseId",
                table: "CharityKafalaPayments",
                column: "KafalaCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaPayments_PaymentMethodId",
                table: "CharityKafalaPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaPayments_SponsorId",
                table: "CharityKafalaPayments",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityKafalaSponsors_SponsorCode",
                table: "CharityKafalaSponsors",
                column: "SponsorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollEmployeeItems_PayrollEmployeeId",
                table: "CharityPayrollEmployeeItems",
                column: "PayrollEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollEmployeeItems_SalaryItemDefinitionId",
                table: "CharityPayrollEmployeeItems",
                column: "SalaryItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollEmployees_EmployeeId",
                table: "CharityPayrollEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollEmployees_PayrollMonthId_EmployeeId",
                table: "CharityPayrollEmployees",
                columns: new[] { "PayrollMonthId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollMonths_Year_Month",
                table: "CharityPayrollMonths",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollPayments_FinancialAccountId",
                table: "CharityPayrollPayments",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollPayments_PaymentMethodId",
                table: "CharityPayrollPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityPayrollPayments_PayrollEmployeeId",
                table: "CharityPayrollPayments",
                column: "PayrollEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivities_ProjectId_Status",
                table: "CharityProjectActivities",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectBeneficiaries_BeneficiaryId",
                table: "CharityProjectBeneficiaries",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectBeneficiaries_ProjectId_BeneficiaryId",
                table: "CharityProjectBeneficiaries",
                columns: new[] { "ProjectId", "BeneficiaryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectBudgetLines_ProjectId_LineType",
                table: "CharityProjectBudgetLines",
                columns: new[] { "ProjectId", "LineType" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectGoals_ProjectId",
                table: "CharityProjectGoals",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectGrants_GrantAgreementId",
                table: "CharityProjectGrants",
                column: "GrantAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectGrants_ProjectId_GrantAgreementId",
                table: "CharityProjectGrants",
                columns: new[] { "ProjectId", "GrantAgreementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseActivities_PhaseId_SortOrder",
                table: "CharityProjectPhaseActivities",
                columns: new[] { "PhaseId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseMilestones_ProjectPhaseId_DueDate",
                table: "CharityProjectPhaseMilestones",
                columns: new[] { "ProjectPhaseId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhases_ProjectId_SortOrder",
                table: "CharityProjectPhases",
                columns: new[] { "ProjectId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseTasks_ActivityId_SortOrder",
                table: "CharityProjectPhaseTasks",
                columns: new[] { "ActivityId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseTasks_PhaseId",
                table: "CharityProjectPhaseTasks",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseTasks_ProjectId_Status",
                table: "CharityProjectPhaseTasks",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalActivitys_ProjectProposalId",
                table: "CharityProjectProposalActivitys",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalAttachments_ProjectProposalId",
                table: "CharityProjectProposalAttachments",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalMonitoringIndicators_ProjectProposalId",
                table: "CharityProjectProposalMonitoringIndicators",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalObjectives_ProjectProposalId",
                table: "CharityProjectProposalObjectives",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalPastExperiences_ProjectProposalId",
                table: "CharityProjectProposalPastExperiences",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposals_ProposalNumber",
                table: "CharityProjectProposals",
                column: "ProposalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalTargetGroups_ProjectProposalId",
                table: "CharityProjectProposalTargetGroups",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalTeamMembers_ProjectProposalId",
                table: "CharityProjectProposalTeamMembers",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalWorkPlans_ProjectProposalId",
                table: "CharityProjectProposalWorkPlans",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjects_Code",
                table: "CharityProjects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjects_Name",
                table: "CharityProjects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjects_Status_IsActive",
                table: "CharityProjects",
                columns: new[] { "Status", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectSubGoalActivities_SubGoalId",
                table: "CharityProjectSubGoalActivities",
                column: "SubGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectSubGoals_GoalId",
                table: "CharityProjectSubGoals",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTaskDailyUpdates_TaskId_UpdateDate",
                table: "CharityProjectTaskDailyUpdates",
                columns: new[] { "TaskId", "UpdateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTrackingLogs_ProjectId_EntryDate",
                table: "CharityProjectTrackingLogs",
                columns: new[] { "ProjectId", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTrackingLogs_ProjectPhaseId",
                table: "CharityProjectTrackingLogs",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharitySalaryItemDefinitions_Name",
                table: "CharitySalaryItemDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityStockDisposalVoucherLines_StockDisposalVoucherId",
                table: "CharityStockDisposalVoucherLines",
                column: "StockDisposalVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStockNeedRequestLines_StockNeedRequestId",
                table: "CharityStockNeedRequestLines",
                column: "StockNeedRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStockReturnVoucherLines_StockReturnVoucherId",
                table: "CharityStockReturnVoucherLines",
                column: "StockReturnVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssueLines_IssueId",
                table: "CharityStoreIssueLines",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssueLines_ItemId",
                table: "CharityStoreIssueLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_ApprovalStatus",
                table: "CharityStoreIssues",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_BeneficiaryId",
                table: "CharityStoreIssues",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_IssueNumber",
                table: "CharityStoreIssues",
                column: "IssueNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_ProjectId",
                table: "CharityStoreIssues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_WarehouseId_IssueDate",
                table: "CharityStoreIssues",
                columns: new[] { "WarehouseId", "IssueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceiptLines_ItemId",
                table: "CharityStoreReceiptLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceiptLines_ReceiptId",
                table: "CharityStoreReceiptLines",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceipts_ApprovalStatus",
                table: "CharityStoreReceipts",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceipts_ReceiptNumber",
                table: "CharityStoreReceipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceipts_WarehouseId_ReceiptDate",
                table: "CharityStoreReceipts",
                columns: new[] { "WarehouseId", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerAvailabilitySlots_VolunteerId",
                table: "CharityVolunteerAvailabilitySlots",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerHourLogs_AssignmentId",
                table: "CharityVolunteerHourLogs",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerHourLogs_ProjectId",
                table: "CharityVolunteerHourLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerHourLogs_VolunteerId_WorkDate",
                table: "CharityVolunteerHourLogs",
                columns: new[] { "VolunteerId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerProjectAssignments_ProjectId",
                table: "CharityVolunteerProjectAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerProjectAssignments_VolunteerId_Status",
                table: "CharityVolunteerProjectAssignments",
                columns: new[] { "VolunteerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteers_NationalId",
                table: "CharityVolunteers",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteers_PhoneNumber",
                table: "CharityVolunteers",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteers_VolunteerCode",
                table: "CharityVolunteers",
                column: "VolunteerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerSkillDefinitions_Name",
                table: "CharityVolunteerSkillDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerSkills_SkillDefinitionId",
                table: "CharityVolunteerSkills",
                column: "SkillDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityVolunteerSkills_VolunteerId_SkillDefinitionId",
                table: "CharityVolunteerSkills",
                columns: new[] { "VolunteerId", "SkillDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Chatid",
                table: "ChatMessage",
                column: "Chatid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_receiver_id",
                table: "ChatMessage",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_RPersonalid",
                table: "ChatMessage",
                column: "RPersonalid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_senderid",
                table: "ChatMessage",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_SPersonalid",
                table: "ChatMessage",
                column: "SPersonalid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_FromUserId",
                table: "ChatMessages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_RoomId_SentAtUtc",
                table: "ChatMessages",
                columns: new[] { "RoomId", "SentAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ToUserId",
                table: "ChatMessages",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_Name",
                table: "ChatRooms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Complainid",
                table: "Comment",
                column: "Complainid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_employerid",
                table: "Comment",
                column: "employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Miloid",
                table: "Comment",
                column: "Miloid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Personalid",
                table: "Comment",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_postid",
                table: "Comment",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Taskid",
                table: "Comment",
                column: "Taskid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Tickid",
                table: "Comment",
                column: "Tickid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_todoid",
                table: "Comment",
                column: "todoid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_userid",
                table: "Comment",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Complain_personald",
                table: "Complain",
                column: "personald");

            migrationBuilder.CreateIndex(
                name: "IX_Complain_senderid",
                table: "Complain",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_Complain_solverid",
                table: "Complain",
                column: "solverid");

            migrationBuilder.CreateIndex(
                name: "IX_Cost_Milestoneid",
                table: "Cost",
                column: "Milestoneid");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Account_id",
                table: "Customer",
                column: "Account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_address id",
                table: "Customer",
                column: "address id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_comid",
                table: "Customer",
                column: "comid");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccountTransactions_CustomerId",
                table: "CustomerAccountTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClients_CustomerNumber",
                table: "CustomerClients",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOldRecords_CustomerId",
                table: "CustomerOldRecords",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_CustomerId",
                table: "CustomerReceipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_PaymentMethodId",
                table: "CustomerReceipts",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_ReceiptNumber",
                table: "CustomerReceipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebitTrans_AccountId",
                table: "DebitTrans",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitTrans_Employeeid",
                table: "DebitTrans",
                column: "Employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_DebitTrans_TreasuryId",
                table: "DebitTrans",
                column: "TreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliverable_MilestoneId",
                table: "Deliverable",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryDevices_DirectoryUserId",
                table: "DirectoryDevices",
                column: "DirectoryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryUsers_AdObjectId",
                table: "DirectoryUsers",
                column: "AdObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryUsers_Upn",
                table: "DirectoryUsers",
                column: "Upn");

            migrationBuilder.CreateIndex(
                name: "IX_Document_Miloid",
                table: "Document",
                column: "Miloid");

            migrationBuilder.CreateIndex(
                name: "IX_Document_proid",
                table: "Document",
                column: "proid");

            migrationBuilder.CreateIndex(
                name: "IX_Document_reqid",
                table: "Document",
                column: "reqid");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ReqResid",
                table: "Document",
                column: "ReqResid");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Companyid",
                table: "Employee",
                column: "Companyid");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Compid",
                table: "Event",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Employerid",
                table: "Event",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Groupid",
                table: "Event",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Personalid",
                table: "Event",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_EventReminders_EventId",
                table: "EventReminders",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_exhib_address_id",
                table: "exhib",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_exhib_Compid",
                table: "exhib",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_exhib_Employerid",
                table: "exhib",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_exhib_Groupid",
                table: "exhib",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_exhib_Personalid",
                table: "exhib",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_exhib products_Product id",
                table: "exhib products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_AccountId",
                table: "Expense",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_Employeeid",
                table: "Expense",
                column: "Employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_TreasuryId",
                table: "Expense",
                column: "TreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_CategoryCode",
                table: "ExpenseCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseNumber",
                table: "Expenses",
                column: "ExpenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentMethodId",
                table: "Expenses",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FinacialAccounts_AccountCode",
                table: "FinacialAccounts",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinacialAccounts_ParentAccountId",
                table: "FinacialAccounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_hours_taskid",
                table: "hours",
                column: "taskid");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeMovements_EmployeeId_EffectiveDate",
                table: "HrEmployeeMovements",
                columns: new[] { "EmployeeId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeMovements_FromDepartmentId",
                table: "HrEmployeeMovements",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeMovements_FromJobTitleId",
                table: "HrEmployeeMovements",
                column: "FromJobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeMovements_ToDepartmentId",
                table: "HrEmployeeMovements",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeMovements_ToJobTitleId",
                table: "HrEmployeeMovements",
                column: "ToJobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrOutRequests_EmployeeId_OutDate",
                table: "HrOutRequests",
                columns: new[] { "EmployeeId", "OutDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HrOutRequests_Status",
                table: "HrOutRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceEvaluations_EmployeeId_EvaluationDate",
                table: "HrPerformanceEvaluations",
                columns: new[] { "EmployeeId", "EvaluationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceEvaluations_EvaluatorEmployeeId",
                table: "HrPerformanceEvaluations",
                column: "EvaluatorEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSanctionRecords_EmployeeId_SanctionDate",
                table: "HrSanctionRecords",
                columns: new[] { "EmployeeId", "SanctionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Addpro_id",
                table: "Identity",
                column: "Addpro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Compid",
                table: "Identity",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Employerid",
                table: "Identity",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_exhib_id",
                table: "Identity",
                column: "exhib_id");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Groupid",
                table: "Identity",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_invent_id",
                table: "Identity",
                column: "invent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Personalid",
                table: "Identity",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RemovePro_id",
                table: "Identity",
                column: "RemovePro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Replacepro_id",
                table: "Identity",
                column: "Replacepro_id");

            migrationBuilder.CreateIndex(
                name: "IX_identity_products_Product id",
                table: "identity_products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_Importer_Account_id",
                table: "Importer",
                column: "Account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Importer_address id",
                table: "Importer",
                column: "address id");

            migrationBuilder.CreateIndex(
                name: "IX_Importer_comid",
                table: "Importer",
                column: "comid");

            migrationBuilder.CreateIndex(
                name: "IX_Income_AccountId",
                table: "Income",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Income_Employeeid",
                table: "Income",
                column: "Employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Income_TreasuryId",
                table: "Income",
                column: "TreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_instructions_TaskId",
                table: "instructions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_invent_address_id",
                table: "invent",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_invent_Compid",
                table: "invent",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_invent_Employerid",
                table: "invent",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_invent_Groupid",
                table: "invent",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_invent_Personalid",
                table: "invent",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_invent products_Product id",
                table: "invent products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_IPAddress_ILineid",
                table: "IPAddress",
                column: "ILineid");

            migrationBuilder.CreateIndex(
                name: "IX_IPAddress_VLineid",
                table: "IPAddress",
                column: "VLineid");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroups_GroupCode",
                table: "ItemGroups",
                column: "GroupCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroups_ParentGroupId",
                table: "ItemGroups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Barcode",
                table: "Items",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemCode",
                table: "Items",
                column: "ItemCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemGroupId",
                table: "Items",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UnitId",
                table: "Items",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemWarehouseBalances_ItemId_WarehouseId",
                table: "ItemWarehouseBalances",
                columns: new[] { "ItemId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemWarehouseBalances_WarehouseId",
                table: "ItemWarehouseBalances",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_kanbanTasks_BoardId_Status_OrderIndex",
                table: "kanbanTasks",
                columns: new[] { "BoardId", "Status", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_CatId",
                table: "Message",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ReceiverId",
                table: "Message",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderID",
                table: "Message",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_objectiveid",
                table: "Milestone",
                column: "objectiveid");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_Project_projectid",
                table: "Milestone",
                column: "Project_projectid");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_statusid",
                table: "Milestone",
                column: "statusid");

            migrationBuilder.CreateIndex(
                name: "IX_miss_products_Product id",
                table: "miss_products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_Compid",
                table: "MissingItem",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_Employerid",
                table: "MissingItem",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_Groupid",
                table: "MissingItem",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_Personalid",
                table: "MissingItem",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_Remove_id",
                table: "MissingItem",
                column: "Remove_id");

            migrationBuilder.CreateIndex(
                name: "IX_MissingItem_RemovePro_id",
                table: "MissingItem",
                column: "RemovePro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Employerid",
                table: "Notification",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Personalid",
                table: "Notification",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId",
                table: "NotificationDeliveries",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_UserId_CreatedAtUtc",
                table: "NotificationDeliveries",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_UserId_IsRead",
                table: "NotificationDeliveries",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedByUserId_IsRead_CreatedAtUtc",
                table: "Notifications",
                columns: new[] { "CreatedByUserId", "IsRead", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Kind",
                table: "Notifications",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Level",
                table: "Notifications",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Objective_Tickid",
                table: "Objective",
                column: "Tickid");

            migrationBuilder.CreateIndex(
                name: "IX_Obstackle_Taskid",
                table: "Obstackle",
                column: "Taskid");

            migrationBuilder.CreateIndex(
                name: "IX_OpticalWorkOrders_CustomerId",
                table: "OpticalWorkOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OpticalWorkOrders_PrescriptionId",
                table: "OpticalWorkOrders",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpticalWorkOrders_SalesInvoiceId",
                table: "OpticalWorkOrders",
                column: "SalesInvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpticalWorkOrders_WorkOrderNumber",
                table: "OpticalWorkOrders",
                column: "WorkOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayedTrans_AccountId",
                table: "PayedTrans",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PayedTrans_Employeeid",
                table: "PayedTrans",
                column: "Employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_PayedTrans_TreasuryId",
                table: "PayedTrans",
                column: "TreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_MethodCode",
                table: "PaymentMethods",
                column: "MethodCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInformation_GroupId",
                table: "PersonalInformation",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInformation_UserId",
                table: "PersonalInformation",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_portif_companyid",
                table: "portif",
                column: "companyid");

            migrationBuilder.CreateIndex(
                name: "IX_portif_Employer_ID",
                table: "portif",
                column: "Employer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_portif_groupid",
                table: "portif",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_PosHoldLines_ItemId",
                table: "PosHoldLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PosHoldLines_PosHoldId",
                table: "PosHoldLines",
                column: "PosHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_PosHolds_HoldNumber",
                table: "PosHolds",
                column: "HoldNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosHolds_PaymentMethodId",
                table: "PosHolds",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PosHolds_WarehouseId",
                table: "PosHolds",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_groupid",
                table: "Post",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Post_personalid",
                table: "Post",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Post_senderid",
                table: "Post",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_CustomerId",
                table: "Prescriptions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Compid",
                table: "Product",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CustomerCustomter_id",
                table: "Product",
                column: "CustomerCustomter_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Employerid",
                table: "Product",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Groupid",
                table: "Product",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_importer id",
                table: "Product",
                column: "importer id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Personalid",
                table: "Product",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_unity id",
                table: "Product",
                column: "unity id");

            migrationBuilder.CreateIndex(
                name: "IX_Project_companyid",
                table: "Project",
                column: "companyid");

            migrationBuilder.CreateIndex(
                name: "IX_Project_personalid",
                table: "Project",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Project_statusid",
                table: "Project",
                column: "statusid");

            migrationBuilder.CreateIndex(
                name: "IX_Project_teamid",
                table: "Project",
                column: "teamid");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccountingProfiles_DefaultCostCenterId",
                table: "ProjectAccountingProfiles",
                column: "DefaultCostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccountingProfiles_DefaultExpenseAccountId",
                table: "ProjectAccountingProfiles",
                column: "DefaultExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccountingProfiles_DefaultRevenueAccountId",
                table: "ProjectAccountingProfiles",
                column: "DefaultRevenueAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccountingProfiles_ProjectId",
                table: "ProjectAccountingProfiles",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenseLinks_CostCenterId",
                table: "ProjectExpenseLinks",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenseLinks_ExpenseId",
                table: "ProjectExpenseLinks",
                column: "ExpenseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenseLinks_ProjectBudgetLineId",
                table: "ProjectExpenseLinks",
                column: "ProjectBudgetLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenseLinks_ProjectId",
                table: "ProjectExpenseLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseExpenseLinks_CostCenterId",
                table: "ProjectPhaseExpenseLinks",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseExpenseLinks_ExpenseId",
                table: "ProjectPhaseExpenseLinks",
                column: "ExpenseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseExpenseLinks_ProjectBudgetLineId",
                table: "ProjectPhaseExpenseLinks",
                column: "ProjectBudgetLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseExpenseLinks_ProjectId_ProjectPhaseId",
                table: "ProjectPhaseExpenseLinks",
                columns: new[] { "ProjectId", "ProjectPhaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseExpenseLinks_ProjectPhaseId",
                table: "ProjectPhaseExpenseLinks",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseStoreIssueLinks_CostCenterId",
                table: "ProjectPhaseStoreIssueLinks",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseStoreIssueLinks_ProjectId_ProjectPhaseId",
                table: "ProjectPhaseStoreIssueLinks",
                columns: new[] { "ProjectId", "ProjectPhaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseStoreIssueLinks_ProjectPhaseId",
                table: "ProjectPhaseStoreIssueLinks",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhaseStoreIssueLinks_StoreIssueId",
                table: "ProjectPhaseStoreIssueLinks",
                column: "StoreIssueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCode",
                table: "Projects",
                column: "ProjectCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceLines_ItemId",
                table: "PurchaseInvoiceLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceLines_PurchaseInvoiceId",
                table: "PurchaseInvoiceLines",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_InvoiceNumber",
                table: "PurchaseInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_SupplierId",
                table: "PurchaseInvoices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_WarehouseId",
                table: "PurchaseInvoices",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases Replace_Compid",
                table: "Purchases Replace",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases Replace_Employerid",
                table: "Purchases Replace",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases Replace_Groupid",
                table: "Purchases Replace",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases Replace_importer id",
                table: "Purchases Replace",
                column: "importer id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases Replace_Personalid",
                table: "Purchases Replace",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchses Bill_Compid",
                table: "Purchses Bill",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchses Bill_Employerid",
                table: "Purchses Bill",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchses Bill_Groupid",
                table: "Purchses Bill",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchses Bill_importer id",
                table: "Purchses Bill",
                column: "importer id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchses Bill_Personalid",
                table: "Purchses Bill",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Receiver_telephonenum",
                table: "Receiver",
                column: "telephonenum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_Compid",
                table: "Remove Product",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_Employerid",
                table: "Remove Product",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_exhib_id",
                table: "Remove Product",
                column: "exhib_id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_Groupid",
                table: "Remove Product",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_invent_id",
                table: "Remove Product",
                column: "invent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_Personalid",
                table: "Remove Product",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_product id",
                table: "Remove Product",
                column: "product id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Product_remove id",
                table: "Remove Product",
                column: "remove id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_Compid",
                table: "Remove Receipt",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_CustomerCustomter_id",
                table: "Remove Receipt",
                column: "CustomerCustomter_id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_Employerid",
                table: "Remove Receipt",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_exhib id",
                table: "Remove Receipt",
                column: "exhib id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_Groupid",
                table: "Remove Receipt",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_importer id",
                table: "Remove Receipt",
                column: "importer id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_invent id",
                table: "Remove Receipt",
                column: "invent id");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_Personalid",
                table: "Remove Receipt",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Remove Receipt_Sales_BillBill_id",
                table: "Remove Receipt",
                column: "Sales_BillBill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_Compid",
                table: "Replace Products",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_Employerid",
                table: "Replace Products",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_exhib id",
                table: "Replace Products",
                column: "exhib id");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_Groupid",
                table: "Replace Products",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_invent id",
                table: "Replace Products",
                column: "invent id");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_Personalid",
                table: "Replace Products",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_Product id",
                table: "Replace Products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_Replace Products_replace id",
                table: "Replace Products",
                column: "replace id");

            migrationBuilder.CreateIndex(
                name: "IX_Request_Depid",
                table: "Request",
                column: "Depid");

            migrationBuilder.CreateIndex(
                name: "IX_Request_employerid",
                table: "Request",
                column: "employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Request_groupid",
                table: "Request",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Request_personalid",
                table: "Request",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_Depid",
                table: "RequestResponse",
                column: "Depid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_employerid",
                table: "RequestResponse",
                column: "employerid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_groupid",
                table: "RequestResponse",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_personalid",
                table: "RequestResponse",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_Reqid",
                table: "RequestResponse",
                column: "Reqid");

            migrationBuilder.CreateIndex(
                name: "IX_Sales Bill_comid",
                table: "Sales Bill",
                column: "comid");

            migrationBuilder.CreateIndex(
                name: "IX_Sales Bill_Customer id",
                table: "Sales Bill",
                column: "Customer id");

            migrationBuilder.CreateIndex(
                name: "IX_Sales Replace_comid",
                table: "Sales Replace",
                column: "comid");

            migrationBuilder.CreateIndex(
                name: "IX_Sales Replace_Customer id",
                table: "Sales Replace",
                column: "Customer id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesBill Products_Bill id",
                table: "SalesBill Products",
                column: "Bill id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesBill Products_Product id",
                table: "SalesBill Products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceLines_ItemId",
                table: "SalesInvoiceLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceLines_SalesInvoiceId",
                table: "SalesInvoiceLines",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoicePayments_PaymentMethodId",
                table: "SalesInvoicePayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoicePayments_SalesInvoiceId",
                table: "SalesInvoicePayments",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_InvoiceNumber",
                table: "SalesInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_PrescriptionId",
                table: "SalesInvoices",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_WarehouseId",
                table: "SalesInvoices",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReplace Products_exhib id",
                table: "SalesReplace Products",
                column: "exhib id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReplace Products_invent id",
                table: "SalesReplace Products",
                column: "invent id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReplace Products_Product id",
                table: "SalesReplace Products",
                column: "Product id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReplace Products_replace id",
                table: "SalesReplace Products",
                column: "replace id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnInvoices_OriginalSalesInvoiceId",
                table: "SalesReturnInvoices",
                column: "OriginalSalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnInvoices_ReturnNumber",
                table: "SalesReturnInvoices",
                column: "ReturnNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnInvoices_WarehouseId",
                table: "SalesReturnInvoices",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnLines_ItemId",
                table: "SalesReturnLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnLines_OriginalSalesInvoiceLineId",
                table: "SalesReturnLines",
                column: "OriginalSalesInvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnLines_SalesReturnInvoiceId",
                table: "SalesReturnLines",
                column: "SalesReturnInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sender_balanceID",
                table: "Sender",
                column: "balanceID");

            migrationBuilder.CreateIndex(
                name: "IX_Sender_CatId",
                table: "Sender",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_Sender_LimitID",
                table: "Sender",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_Session_connectionID",
                table: "Session",
                column: "connectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Stock Taking_Compid",
                table: "Stock Taking",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Stock Taking_Employerid",
                table: "Stock Taking",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Stock Taking_Groupid",
                table: "Stock Taking",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Stock Taking_Personalid",
                table: "Stock Taking",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ItemId",
                table: "StockTransactions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_RelatedWarehouseId",
                table: "StockTransactions",
                column: "RelatedWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierNumber",
                table: "Suppliers",
                column: "SupplierNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_ADSlid",
                table: "SystemComponent",
                column: "ADSlid");

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_GPSid",
                table: "SystemComponent",
                column: "GPSid");

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_Intenetlineid",
                table: "SystemComponent",
                column: "Intenetlineid");

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_USBMid",
                table: "SystemComponent",
                column: "USBMid");

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_userid",
                table: "SystemComponent",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_SystemComponent_VPNlineid",
                table: "SystemComponent",
                column: "VPNlineid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_employeeid",
                table: "Task",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_milestoneid",
                table: "Task",
                column: "milestoneid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Reqid",
                table: "Task",
                column: "Reqid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Tickid",
                table: "Task",
                column: "Tickid");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAudits_TaskId_AtUtc",
                table: "TaskAudits",
                columns: new[] { "TaskId", "AtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_BoardId_Status",
                table: "TaskItems",
                columns: new[] { "BoardId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_employee_id",
                table: "TeamMember",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_role_id",
                table: "TeamMember",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_Tasks_Taskid",
                table: "TeamMember",
                column: "Tasks_Taskid");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_teamid",
                table: "TeamMember",
                column: "teamid");

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_groupid",
                table: "ToDo",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_personalid",
                table: "ToDo",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_senderid",
                table: "ToDo",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction2_Compid",
                table: "Transaction2",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction2_Employerid",
                table: "Transaction2",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction2_Groupid",
                table: "Transaction2",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction2_Personalid",
                table: "Transaction2",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction2_stocktake id",
                table: "Transaction2",
                column: "stocktake id");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_catid",
                table: "TroubleTicket",
                column: "catid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_depid",
                table: "TroubleTicket",
                column: "depid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_groupid",
                table: "TroubleTicket",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_issuerUserID",
                table: "TroubleTicket",
                column: "issuerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_personalid",
                table: "TroubleTicket",
                column: "personalid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_Reqid",
                table: "TroubleTicket",
                column: "Reqid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_statusid",
                table: "TroubleTicket",
                column: "statusid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_syscomid",
                table: "TroubleTicket",
                column: "syscomid");

            migrationBuilder.CreateIndex(
                name: "IX_TroubleTicket_typid",
                table: "TroubleTicket",
                column: "typid");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitCode",
                table: "Units",
                column: "UnitCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unity_Compid",
                table: "Unity",
                column: "Compid");

            migrationBuilder.CreateIndex(
                name: "IX_Unity_Employerid",
                table: "Unity",
                column: "Employerid");

            migrationBuilder.CreateIndex(
                name: "IX_Unity_Groupid",
                table: "Unity",
                column: "Groupid");

            migrationBuilder.CreateIndex(
                name: "IX_Unity_Personalid",
                table: "Unity",
                column: "Personalid");

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_UserId",
                table: "UserLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_employeeid",
                table: "Users",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_profileid",
                table: "Users",
                column: "profileid");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_WarehouseCode",
                table: "Warehouses",
                column: "WarehouseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_AssignedRole_Status",
                table: "WorkflowSteps",
                columns: new[] { "AssignedRole", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_AssignedRole_Status_IsActive_CreatedAtUtc",
                table: "WorkflowSteps",
                columns: new[] { "AssignedRole", "Status", "IsActive", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_EntityType_EntityId",
                table: "WorkflowSteps",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_EntityType_EntityId_IsActive_StepOrder",
                table: "WorkflowSteps",
                columns: new[] { "EntityType", "EntityId", "IsActive", "StepOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_Add product_Add Receipt_add id",
                table: "Add product",
                column: "add id",
                principalTable: "Add Receipt",
                principalColumn: "add id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add product_PersonalInformation_Personalid",
                table: "Add product",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add product_Product_Product id",
                table: "Add product",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_Customer_CustomerCustomter_id",
                table: "Add Receipt",
                column: "CustomerCustomter_id",
                principalTable: "Customer",
                principalColumn: "Customer id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_Importer_importer id",
                table: "Add Receipt",
                column: "importer id",
                principalTable: "Importer",
                principalColumn: "importer id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_PersonalInformation_Personalid",
                table: "Add Receipt",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_Purchses Bill_bill id",
                table: "Add Receipt",
                column: "bill id",
                principalTable: "Purchses Bill",
                principalColumn: "Bill id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_exhib_exhib id",
                table: "Add Receipt",
                column: "exhib id",
                principalTable: "exhib",
                principalColumn: "exhib id");

            migrationBuilder.AddForeignKey(
                name: "FK_Add Receipt_invent_invent id",
                table: "Add Receipt",
                column: "invent id",
                principalTable: "invent",
                principalColumn: "invent id");

            migrationBuilder.AddForeignKey(
                name: "FK_address_PersonalInformation_Personalid",
                table: "address",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BeneficiaryContactLogs_CharityBeneficiaries_BeneficiaryId",
                table: "BeneficiaryContactLogs",
                column: "BeneficiaryId",
                principalTable: "CharityBeneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bill Products_PersonalInformation_Personalid",
                table: "Bill Products",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill Products_Product_Product id",
                table: "Bill Products",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bill Products_Purchses Bill_Bill id",
                table: "Bill Products",
                column: "Bill id",
                principalTable: "Purchses Bill",
                principalColumn: "Bill id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAttendees_CalendarEvents_EventId",
                table: "CalendarAttendees",
                column: "EventId",
                principalTable: "CalendarEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAttendees_Users_UserId",
                table: "CalendarAttendees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Users_CreatedByUserId",
                table: "CalendarEvents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidCycleBeneficiaries_CharityAidCycles_AidCycleId",
                table: "CharityAidCycleBeneficiaries",
                column: "AidCycleId",
                principalTable: "CharityAidCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidCycleBeneficiaries_CharityBeneficiaries_BeneficiaryId",
                table: "CharityAidCycleBeneficiaries",
                column: "BeneficiaryId",
                principalTable: "CharityBeneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidCycleBeneficiaries_CharityBeneficiaryCommitteeDecisions_CommitteeDecisionId",
                table: "CharityAidCycleBeneficiaries",
                column: "CommitteeDecisionId",
                principalTable: "CharityBeneficiaryCommitteeDecisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidCycles_Users_ApprovedByUserId",
                table: "CharityAidCycles",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidCycles_Users_CreatedByUserId",
                table: "CharityAidCycles",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursementFundingLines_CharityAidDisbursements_DisbursementId",
                table: "CharityAidDisbursementFundingLines",
                column: "DisbursementId",
                principalTable: "CharityAidDisbursements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursementFundingLines_CharityDonationAllocations_DonationAllocationId",
                table: "CharityAidDisbursementFundingLines",
                column: "DonationAllocationId",
                principalTable: "CharityDonationAllocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursementFundingLines_Users_CreatedByUserId",
                table: "CharityAidDisbursementFundingLines",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_CharityAidRequests_AidRequestId",
                table: "CharityAidDisbursements",
                column: "AidRequestId",
                principalTable: "CharityAidRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_CharityBeneficiaries_BeneficiaryId",
                table: "CharityAidDisbursements",
                column: "BeneficiaryId",
                principalTable: "CharityBeneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_Users_ApprovedByUserId",
                table: "CharityAidDisbursements",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_Users_CreatedByUserId",
                table: "CharityAidDisbursements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_Users_ExecutedByUserId",
                table: "CharityAidDisbursements",
                column: "ExecutedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_Users_RejectedByUserId",
                table: "CharityAidDisbursements",
                column: "RejectedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidRequests_CharityBeneficiaries_BeneficiaryId",
                table: "CharityAidRequests",
                column: "BeneficiaryId",
                principalTable: "CharityBeneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidRequests_Users_CreatedByUserId",
                table: "CharityAidRequests",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityBeneficiaries_Users_CreatedByUserId",
                table: "CharityBeneficiaries",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityBeneficiaryAssessments_Users_ResearcherUserId",
                table: "CharityBeneficiaryAssessments",
                column: "ResearcherUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityBeneficiaryCommitteeDecisions_Users_ApprovedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityBeneficiaryOldRecords_Users_CreatedByUserId",
                table: "CharityBeneficiaryOldRecords",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityDonationAllocations_CharityDonationInKindItems_DonationInKindItemId",
                table: "CharityDonationAllocations",
                column: "DonationInKindItemId",
                principalTable: "CharityDonationInKindItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityDonationAllocations_CharityDonations_DonationId",
                table: "CharityDonationAllocations",
                column: "DonationId",
                principalTable: "CharityDonations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityDonationInKindItems_CharityDonations_DonationId",
                table: "CharityDonationInKindItems",
                column: "DonationId",
                principalTable: "CharityDonations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityDonations_Users_CreatedByUserId",
                table: "CharityDonations",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_PersonalInformation_RPersonalid",
                table: "ChatMessage",
                column: "RPersonalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_PersonalInformation_SPersonalid",
                table: "ChatMessage",
                column: "SPersonalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_FromUserId",
                table: "ChatMessages",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_ToUserId",
                table: "ChatMessages",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Complain_Complainid",
                table: "Comment",
                column: "Complainid",
                principalTable: "Complain",
                principalColumn: "Complain ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Milestone_Miloid",
                table: "Comment",
                column: "Miloid",
                principalTable: "Milestone",
                principalColumn: "Milestoneid");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_PersonalInformation_Personalid",
                table: "Comment",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_postid",
                table: "Comment",
                column: "postid",
                principalTable: "Post",
                principalColumn: "Post ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Task_Taskid",
                table: "Comment",
                column: "Taskid",
                principalTable: "Task",
                principalColumn: "Taskid");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ToDo_todoid",
                table: "Comment",
                column: "todoid",
                principalTable: "ToDo",
                principalColumn: "ToDo ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_TroubleTicket_Tickid",
                table: "Comment",
                column: "Tickid",
                principalTable: "TroubleTicket",
                principalColumn: "Ticketid");

            migrationBuilder.AddForeignKey(
                name: "FK_Complain_PersonalInformation_personald",
                table: "Complain",
                column: "personald",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_Milestone_Milestoneid",
                table: "Cost",
                column: "Milestoneid",
                principalTable: "Milestone",
                principalColumn: "Milestoneid");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliverable_Milestone_MilestoneId",
                table: "Deliverable",
                column: "MilestoneId",
                principalTable: "Milestone",
                principalColumn: "Milestoneid");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Milestone_Miloid",
                table: "Document",
                column: "Miloid",
                principalTable: "Milestone",
                principalColumn: "Milestoneid");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Project_proid",
                table: "Document",
                column: "proid",
                principalTable: "Project",
                principalColumn: "projectid");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_RequestResponse_ReqResid",
                table: "Document",
                column: "ReqResid",
                principalTable: "RequestResponse",
                principalColumn: "RequestRes ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Request_reqid",
                table: "Document",
                column: "reqid",
                principalTable: "Request",
                principalColumn: "Request ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_PersonalInformation_Personalid",
                table: "Event",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_exhib_PersonalInformation_Personalid",
                table: "exhib",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_exhib products_Product_Product id",
                table: "exhib products",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_hours_Task_taskid",
                table: "hours",
                column: "taskid",
                principalTable: "Task",
                principalColumn: "Taskid");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_PersonalInformation_Personalid",
                table: "Identity",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Remove Product_RemovePro_id",
                table: "Identity",
                column: "RemovePro_id",
                principalTable: "Remove Product",
                principalColumn: " RemovePro id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Replace Products_Replacepro_id",
                table: "Identity",
                column: "Replacepro_id",
                principalTable: "Replace Products",
                principalColumn: "ReplacePro id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_invent_invent_id",
                table: "Identity",
                column: "invent_id",
                principalTable: "invent",
                principalColumn: "invent id");

            migrationBuilder.AddForeignKey(
                name: "FK_identity_products_Product_Product id",
                table: "identity_products",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_instructions_Task_TaskId",
                table: "instructions",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Taskid");

            migrationBuilder.AddForeignKey(
                name: "FK_invent_PersonalInformation_Personalid",
                table: "invent",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invent products_Product_Product id",
                table: "invent products",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Milestone_Objective_objectiveid",
                table: "Milestone",
                column: "objectiveid",
                principalTable: "Objective",
                principalColumn: "objectiveid");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestone_Project_Project_projectid",
                table: "Milestone",
                column: "Project_projectid",
                principalTable: "Project",
                principalColumn: "projectid");

            migrationBuilder.AddForeignKey(
                name: "FK_miss_products_MissingItem_miss id",
                table: "miss_products",
                column: "miss id",
                principalTable: "MissingItem",
                principalColumn: "miss id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_miss_products_Product_Product id",
                table: "miss_products",
                column: "Product id",
                principalTable: "Product",
                principalColumn: "Product ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissingItem_PersonalInformation_Personalid",
                table: "MissingItem",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissingItem_Remove Product_RemovePro_id",
                table: "MissingItem",
                column: "RemovePro_id",
                principalTable: "Remove Product",
                principalColumn: " RemovePro id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissingItem_Remove Receipt_Remove_id",
                table: "MissingItem",
                column: "Remove_id",
                principalTable: "Remove Receipt",
                principalColumn: "Remove id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_PersonalInformation_Personalid",
                table: "Notification",
                column: "Personalid",
                principalTable: "PersonalInformation",
                principalColumn: "person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDeliveries_Notifications_NotificationId",
                table: "NotificationDeliveries",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDeliveries_Users_UserId",
                table: "NotificationDeliveries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_CreatedByUserId",
                table: "Notifications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_TroubleTicket_Tickid",
                table: "Objective",
                column: "Tickid",
                principalTable: "TroubleTicket",
                principalColumn: "Ticketid");

            migrationBuilder.AddForeignKey(
                name: "FK_Obstackle_Task_Taskid",
                table: "Obstackle",
                column: "Taskid",
                principalTable: "Task",
                principalColumn: "Taskid");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInformation_Users_UserId",
                table: "PersonalInformation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Employee_employeeid",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInformation_Group_GroupId",
                table: "PersonalInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_PersonalInformation_profileid",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AccountingIntegrationProfiles");

            migrationBuilder.DropTable(
                name: "AccountingIntegrationSourceDefinitions");

            migrationBuilder.DropTable(
                name: "AccountingJournalEntryLines");

            migrationBuilder.DropTable(
                name: "AccountingPostingProfiles");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BackgroundJob");

            migrationBuilder.DropTable(
                name: "BeneficiaryContactLogs");

            migrationBuilder.DropTable(
                name: "Bill Products");

            migrationBuilder.DropTable(
                name: "boardUsers");

            migrationBuilder.DropTable(
                name: "CalendarAttendees");

            migrationBuilder.DropTable(
                name: "CharityActivityPhaseAssignments");

            migrationBuilder.DropTable(
                name: "CharityAidCycleBeneficiaries");

            migrationBuilder.DropTable(
                name: "CharityAidDisbursementFundingLines");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryAssessments");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryDocuments");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryFamilyMembers");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchCommitteeEvaluations");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchDebts");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchExpenseItems");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchFamilyMembers");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchHouseAssets");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchIncomeItems");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchReviews");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryOldRecords");

            migrationBuilder.DropTable(
                name: "CharityBoardDecisionAttachments");

            migrationBuilder.DropTable(
                name: "CharityBoardDecisionFollowUps");

            migrationBuilder.DropTable(
                name: "CharityBoardMeetingAttachments");

            migrationBuilder.DropTable(
                name: "CharityBoardMeetingAttendees");

            migrationBuilder.DropTable(
                name: "CharityBoardMeetingMinutes");

            migrationBuilder.DropTable(
                name: "CharityEmployeeSalaryStructures");

            migrationBuilder.DropTable(
                name: "CharityGrantConditions");

            migrationBuilder.DropTable(
                name: "CharityGrantInstallments");

            migrationBuilder.DropTable(
                name: "CharityHrAttendanceRecords");

            migrationBuilder.DropTable(
                name: "CharityHrEmployeeBonuses");

            migrationBuilder.DropTable(
                name: "CharityHrEmployeeContracts");

            migrationBuilder.DropTable(
                name: "CharityHrEmployeeFundingAssignments");

            migrationBuilder.DropTable(
                name: "CharityHrEmployeeTaskAssignments");

            migrationBuilder.DropTable(
                name: "CharityKafalaPayments");

            migrationBuilder.DropTable(
                name: "CharityPayrollEmployeeItems");

            migrationBuilder.DropTable(
                name: "CharityPayrollPayments");

            migrationBuilder.DropTable(
                name: "CharityProjectActivities");

            migrationBuilder.DropTable(
                name: "CharityProjectBeneficiaries");

            migrationBuilder.DropTable(
                name: "CharityProjectGrants");

            migrationBuilder.DropTable(
                name: "CharityProjectPhaseMilestones");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalActivitys");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalAttachments");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalMonitoringIndicators");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalObjectives");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalPastExperiences");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalTargetGroups");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalTeamMembers");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalWorkPlans");

            migrationBuilder.DropTable(
                name: "CharityProjectTaskDailyUpdates");

            migrationBuilder.DropTable(
                name: "CharityProjectTrackingLogs");

            migrationBuilder.DropTable(
                name: "CharityStockDisposalVoucherLines");

            migrationBuilder.DropTable(
                name: "CharityStockNeedRequestLines");

            migrationBuilder.DropTable(
                name: "CharityStockReturnVoucherLines");

            migrationBuilder.DropTable(
                name: "CharityStoreIssueLines");

            migrationBuilder.DropTable(
                name: "CharityStoreReceiptLines");

            migrationBuilder.DropTable(
                name: "CharityVolunteerAvailabilitySlots");

            migrationBuilder.DropTable(
                name: "CharityVolunteerHourLogs");

            migrationBuilder.DropTable(
                name: "CharityVolunteerSkills");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "Cost");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "CustomerAccountTransactions");

            migrationBuilder.DropTable(
                name: "CustomerOldRecords");

            migrationBuilder.DropTable(
                name: "CustomerReceipts");

            migrationBuilder.DropTable(
                name: "DebitTrans");

            migrationBuilder.DropTable(
                name: "Deliverable");

            migrationBuilder.DropTable(
                name: "DhcpRecords");

            migrationBuilder.DropTable(
                name: "DirectoryDevices");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Error");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "EventItem");

            migrationBuilder.DropTable(
                name: "EventReminders");

            migrationBuilder.DropTable(
                name: "exhib products");

            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "hours");

            migrationBuilder.DropTable(
                name: "HrEmployeeMovements");

            migrationBuilder.DropTable(
                name: "HrOutRequests");

            migrationBuilder.DropTable(
                name: "HrPerformanceEvaluations");

            migrationBuilder.DropTable(
                name: "HrSanctionRecords");

            migrationBuilder.DropTable(
                name: "identity_products");

            migrationBuilder.DropTable(
                name: "Income");

            migrationBuilder.DropTable(
                name: "instructions");

            migrationBuilder.DropTable(
                name: "invent products");

            migrationBuilder.DropTable(
                name: "IPAddress");

            migrationBuilder.DropTable(
                name: "ItemWarehouseBalances");

            migrationBuilder.DropTable(
                name: "kanbanTasks");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "miss_products");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "NotificationDeliveries");

            migrationBuilder.DropTable(
                name: "Obstackle");

            migrationBuilder.DropTable(
                name: "Onlineuser");

            migrationBuilder.DropTable(
                name: "OpticalWorkOrders");

            migrationBuilder.DropTable(
                name: "PayedTrans");

            migrationBuilder.DropTable(
                name: "portfolio");

            migrationBuilder.DropTable(
                name: "portif");

            migrationBuilder.DropTable(
                name: "PosHoldLines");

            migrationBuilder.DropTable(
                name: "ProjectAccountingProfiles");

            migrationBuilder.DropTable(
                name: "ProjectExpenseLinks");

            migrationBuilder.DropTable(
                name: "ProjectPhaseExpenseLinks");

            migrationBuilder.DropTable(
                name: "ProjectPhaseStoreIssueLinks");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceLines");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "SalesBill Products");

            migrationBuilder.DropTable(
                name: "SalesInvoicePayments");

            migrationBuilder.DropTable(
                name: "SalesReplace Products");

            migrationBuilder.DropTable(
                name: "SalesReturnLines");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "TaskAudits");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Transaction2");

            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "AccountingJournalEntries");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CharityProjectSubGoalActivities");

            migrationBuilder.DropTable(
                name: "CharityAidCycles");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropTable(
                name: "CharityAidDisbursements");

            migrationBuilder.DropTable(
                name: "CharityDonationAllocations");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryHumanitarianResearchs");

            migrationBuilder.DropTable(
                name: "CharityBoardDecisions");

            migrationBuilder.DropTable(
                name: "CharityHrShifts");

            migrationBuilder.DropTable(
                name: "CharityKafalaCases");

            migrationBuilder.DropTable(
                name: "CharitySalaryItemDefinitions");

            migrationBuilder.DropTable(
                name: "CharityPayrollEmployees");

            migrationBuilder.DropTable(
                name: "CharityGrantAgreements");

            migrationBuilder.DropTable(
                name: "CharityProjectProposals");

            migrationBuilder.DropTable(
                name: "CharityProjectPhaseTasks");

            migrationBuilder.DropTable(
                name: "CharityStockDisposalVouchers");

            migrationBuilder.DropTable(
                name: "CharityStockNeedRequests");

            migrationBuilder.DropTable(
                name: "CharityStockReturnVouchers");

            migrationBuilder.DropTable(
                name: "CharityStoreReceipts");

            migrationBuilder.DropTable(
                name: "CharityVolunteerProjectAssignments");

            migrationBuilder.DropTable(
                name: "CharityVolunteerSkillDefinitions");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "Complain");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "ToDo");

            migrationBuilder.DropTable(
                name: "DirectoryUsers");

            migrationBuilder.DropTable(
                name: "RequestResponse");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "Receiver");

            migrationBuilder.DropTable(
                name: "Sender");

            migrationBuilder.DropTable(
                name: "MissingItem");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PosHolds");

            migrationBuilder.DropTable(
                name: "CharityProjectBudgetLines");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "AccountingCostCenters");

            migrationBuilder.DropTable(
                name: "CharityStoreIssues");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices");

            migrationBuilder.DropTable(
                name: "Sales Replace");

            migrationBuilder.DropTable(
                name: "SalesInvoiceLines");

            migrationBuilder.DropTable(
                name: "SalesReturnInvoices");

            migrationBuilder.DropTable(
                name: "connection");

            migrationBuilder.DropTable(
                name: "TaskBoards");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Stock Taking");

            migrationBuilder.DropTable(
                name: "AccountingFiscalPeriods");

            migrationBuilder.DropTable(
                name: "CharityProjectSubGoals");

            migrationBuilder.DropTable(
                name: "CharityAidRequests");

            migrationBuilder.DropTable(
                name: "CharityDonationInKindItems");

            migrationBuilder.DropTable(
                name: "CharityBoardMeetings");

            migrationBuilder.DropTable(
                name: "CharityKafalaSponsors");

            migrationBuilder.DropTable(
                name: "CharityHrEmployees");

            migrationBuilder.DropTable(
                name: "CharityPayrollMonths");

            migrationBuilder.DropTable(
                name: "CharityFunders");

            migrationBuilder.DropTable(
                name: "CharityProjectPhaseActivities");

            migrationBuilder.DropTable(
                name: "CharityVolunteers");

            migrationBuilder.DropTable(
                name: "Add product");

            migrationBuilder.DropTable(
                name: "Replace Products");

            migrationBuilder.DropTable(
                name: "Remove Product");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "Milestone");

            migrationBuilder.DropTable(
                name: "CharityProjectGoals");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaries");

            migrationBuilder.DropTable(
                name: "CharityDonations");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "CharityHrDepartments");

            migrationBuilder.DropTable(
                name: "CharityHrJobTitles");

            migrationBuilder.DropTable(
                name: "CharityProjectPhases");

            migrationBuilder.DropTable(
                name: "Add Receipt");

            migrationBuilder.DropTable(
                name: "Purchases Replace");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Remove Receipt");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Objective");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryStatuses");

            migrationBuilder.DropTable(
                name: "CharityGenders");

            migrationBuilder.DropTable(
                name: "CharityMaritalStatuses");

            migrationBuilder.DropTable(
                name: "CharityAidTypes");

            migrationBuilder.DropTable(
                name: "CharityDonors");

            migrationBuilder.DropTable(
                name: "FinacialAccounts");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ItemGroups");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "CharityProjects");

            migrationBuilder.DropTable(
                name: "Purchses Bill");

            migrationBuilder.DropTable(
                name: "Unity");

            migrationBuilder.DropTable(
                name: "Sales Bill");

            migrationBuilder.DropTable(
                name: "exhib");

            migrationBuilder.DropTable(
                name: "invent");

            migrationBuilder.DropTable(
                name: "CustomerClients");

            migrationBuilder.DropTable(
                name: "TroubleTicket");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "CharityAreas");

            migrationBuilder.DropTable(
                name: "Importer");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "SystemComponent");

            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.DropTable(
                name: "CharityCities");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "Balance");

            migrationBuilder.DropTable(
                name: "Limit");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "ADSL_Line");

            migrationBuilder.DropTable(
                name: "DomainUser");

            migrationBuilder.DropTable(
                name: "GPS_Tracking_Car");

            migrationBuilder.DropTable(
                name: "InternetLeasedLine");

            migrationBuilder.DropTable(
                name: "USB_Modem");

            migrationBuilder.DropTable(
                name: "VPNLeasedLine");

            migrationBuilder.DropTable(
                name: "CharityGovernorates");

            migrationBuilder.DropTable(
                name: "Treasury");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "PersonalInformation");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
