using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class OpticalItemOptionVm
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
    }
}
