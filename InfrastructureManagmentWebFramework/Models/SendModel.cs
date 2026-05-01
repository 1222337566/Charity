using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models
{
     public partial record SendModel
    {
        public int? account_id { get; set; }
        public string text { get; set; }
       //public byte dlr_type { get; set; }
       public string sender { get; set; }
        public string msisdn { get; set; }

        public string mnc
        { get; set; }

        public string mcc { get; set; }
     }
}
