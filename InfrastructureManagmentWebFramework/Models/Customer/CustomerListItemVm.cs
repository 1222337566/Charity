using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Customer
{
    public class CustomerListItemVm
    {
        public Guid Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string GenderText { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Tel { get; set; }
        public string? MobileNo { get; set; }
        public bool IsActive { get; set; }
    }
}
