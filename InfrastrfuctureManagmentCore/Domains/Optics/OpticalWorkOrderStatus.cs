using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Optics
{
    public enum OpticalWorkOrderStatus
    {
        New = 1,
        InLab = 2,
        Ready = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
