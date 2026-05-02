using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models
{
    public partial record ReceiveModel
    {
        public Int64 message_id { get; set; }
public bool status { get; set; }
public string status_description { get; set; }
public DateTime time_stamp { get; set; }
 public int message_parts { get; set; }
    }
}
