using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.PosHolds
{
    public enum PosHoldStatus
    {
        Held = 1,
        Resumed = 2,
        Cancelled = 3,
        Completed = 4
    }
}
