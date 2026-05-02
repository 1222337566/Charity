using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models
{
    public partial record HTSSendSMSModel
    {
        public string  Telephonenumber { get; set; }


        public string message { get; set; } 

    }
}
