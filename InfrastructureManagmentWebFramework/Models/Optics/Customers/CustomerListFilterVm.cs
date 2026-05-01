using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Customers
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CustomerListFilterVm
    {
        public string? Q { get; set; }
        public bool? IsActive { get; set; }
    }
}
