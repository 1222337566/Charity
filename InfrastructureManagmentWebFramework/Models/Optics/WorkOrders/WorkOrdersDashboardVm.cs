using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.WorkOrders
{
    public class WorkOrdersDashboardVm
    {
        public int NewCount { get; set; }
        public int InLabCount { get; set; }
        public int ReadyCount { get; set; }
        public int DeliveredTodayCount { get; set; }
        public int OverdueCount { get; set; }

        public List<OpticalWorkOrderDashboardItemVm> Orders { get; set; } = new();
    }
}
