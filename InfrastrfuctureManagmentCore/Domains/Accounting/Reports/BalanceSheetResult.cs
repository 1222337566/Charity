using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class BalanceSheetResult
    {
        public Guid? FiscalPeriodId { get; set; }
        public DateTime? AsOfDate { get; set; }

        public List<BalanceSheetRow> AssetRows { get; set; } = new();
        public List<BalanceSheetRow> LiabilityRows { get; set; } = new();
        public List<BalanceSheetRow> EquityRows { get; set; } = new();

        public decimal CurrentPeriodSurplusOrDeficit { get; set; }

        public decimal TotalAssets => AssetRows.Sum(x => x.Balance);
        public decimal TotalLiabilities => LiabilityRows.Sum(x => x.Balance);
        public decimal TotalEquityBeforeSurplus => EquityRows.Sum(x => x.Balance);
        public decimal TotalEquityAfterSurplus => TotalEquityBeforeSurplus + CurrentPeriodSurplusOrDeficit;
        public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquityAfterSurplus;
        public decimal BalanceDifference => TotalAssets - TotalLiabilitiesAndEquity;
    }
}
