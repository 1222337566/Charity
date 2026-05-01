using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Requests
{
    public  class Requests:BaseEntity
    {

        public Requests() { }
        public string HostIP { get; set; }
        public string URI { get; set; }
        public string querystring { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string response { get; set; }
        
        public DateTime startReqTime { get; set; }

        public DateTime endReqTime { get; set;}
    }
}
