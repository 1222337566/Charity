using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.Funding
{
    public class GrantorFinancialReportPageVm
    {
        public Guid? GrantorId { get; set; }

        public Guid? ProjectId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<SelectListItem> Grantors { get; set; } = new();

        public List<SelectListItem> Projects { get; set; } = new();

        public List<GrantorFinancialAgreementRowVm> Agreements { get; set; } = new();

        public List<GrantorFinancialBudgetRowVm> BudgetRows { get; set; } = new();

        public List<GrantorFinancialStatementRowVm> StatementRows { get; set; } = new();

        public decimal TotalFunding => Agreements.Sum(x => x.FundingAmount);

        public decimal TotalReceived => Agreements.Sum(x => x.ReceivedAmount);

        public decimal TotalActualExpenses => StatementRows.Where(x => x.IncludeInActualCost).Sum(x => x.Amount);

        public decimal RemainingFunding => TotalFunding - TotalActualExpenses;

        public decimal SpendingPercent => TotalFunding <= 0 ? 0 : Math.Round((TotalActualExpenses / TotalFunding) * 100, 2);
    }

    public class GrantorFinancialAgreementRowVm
    {
        public Guid AgreementId { get; set; }

        public string AgreementNumber { get; set; } = string.Empty;

        public string GrantorName { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public decimal FundingAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        public decimal RemainingToReceive => FundingAmount - ReceivedAmount;

        public DateTime StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? ContactEmail { get; set; }
    }

    public class GrantorFinancialBudgetRowVm
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public Guid? BudgetLineId { get; set; }

        public string BudgetLineName { get; set; } = string.Empty;

        public decimal PlannedAmount { get; set; }

        public decimal ActualAmount { get; set; }

        public decimal RemainingAmount => PlannedAmount - ActualAmount;

        public decimal UtilizationPercent => PlannedAmount <= 0 ? 0 : Math.Round((ActualAmount / PlannedAmount) * 100, 2);
    }

    public class GrantorFinancialStatementRowVm
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string GrantorName { get; set; } = string.Empty;

        public string AgreementNumber { get; set; } = string.Empty;

        public Guid? BudgetLineId { get; set; }

        public string? BudgetLineName { get; set; }

        public string? PhaseName { get; set; }

        public string? CostCenterName { get; set; }

        public DateTime DocumentDateUtc { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string SourceKind { get; set; } = string.Empty;

        public string SourceKindAr { get; set; } = string.Empty;

        public string ExpenseItem { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public bool IncludeInActualCost { get; set; } = true;

        public string? Notes { get; set; }
    }
}
