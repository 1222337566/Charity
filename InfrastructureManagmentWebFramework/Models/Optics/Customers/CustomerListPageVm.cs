using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Customers
{
    public class CustomerListPageVm
    {
        public CustomerListFilterVm Filter { get; set; } = new();
        public List<CustomerListRowVm> Rows { get; set; } = new();

        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int HasBalanceCount { get; set; }
    }
}
