using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Search
{
    public class FindPurchasePageVm
    {
        public FindPurchaseFilterVm Filter { get; set; } = new();
        public List<FindPurchaseRowVm> Rows { get; set; } = new();

        public int Count { get; set; }
        public decimal TotalNetAmount { get; set; }
    }
}
