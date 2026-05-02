using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.ProjectFinancialControl
{
    public class ProjectFinancialControlPageVm
    {
        public Guid? ProjectId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string? SourceKind { get; set; }

        public List<SelectListItem> Projects { get; set; } = new();

        public List<SelectListItem> SourceKinds { get; set; } = new();

        public List<ProjectFinancialSummaryRowVm> Summaries { get; set; } = new();

        public List<ProjectBudgetUtilizationRowVm> BudgetRows { get; set; } = new();

        public List<ProjectExpenseStatementRowVm> StatementRows { get; set; } = new();

        public decimal TotalBudget => Summaries.Sum(x => x.ProjectBudget);

        public decimal TotalActual => Summaries.Sum(x => x.TotalActualCost);

        public decimal TotalRemaining => TotalBudget - TotalActual;
    }

    public class ProjectFinancialSummaryRowVm
    {
        public Guid ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public decimal ProjectBudget { get; set; }

        public decimal DirectCashExpenses { get; set; }

        public decimal PhaseCashExpenses { get; set; }

        public decimal StoreIssueExpenses { get; set; }

        public decimal TotalActualCost => DirectCashExpenses + PhaseCashExpenses + StoreIssueExpenses;

        public decimal RemainingBudget => ProjectBudget - TotalActualCost;

        public decimal UtilizationPercent => ProjectBudget <= 0 ? 0 : Math.Round((TotalActualCost / ProjectBudget) * 100, 2);
    }

    public class ProjectBudgetUtilizationRowVm
    {
        public Guid ProjectId { get; set; }

        public Guid BudgetLineId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string BudgetLineName { get; set; } = string.Empty;

        public string? LineType { get; set; }

        public decimal PlannedAmount { get; set; }

        public decimal ActualAmount { get; set; }

        public decimal RemainingAmount => PlannedAmount - ActualAmount;

        public decimal UtilizationPercent => PlannedAmount <= 0 ? 0 : Math.Round((ActualAmount / PlannedAmount) * 100, 2);
    }

    public class ProjectExpenseStatementRowVm
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public Guid? PhaseId { get; set; }

        public string? PhaseName { get; set; }

        public Guid? BudgetLineId { get; set; }

        public string? BudgetLineName { get; set; }

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
