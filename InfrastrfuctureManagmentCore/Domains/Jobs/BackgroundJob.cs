using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Jobs
{
    public  class BackgroundJob : BaseEntity
    {
        public BackgroundJob() { }
        public string description { get; set; }

        public string requestbody { get; set; }

        public short requestType  { get; set; }

        public string  requestheader { get; set; }

        public string requestmethod { get; set; }


        public DateTime DateTime { get; set; }

        public string requesturl { get; set; }
        
        public string responsBody { get; set; }


        public string responseheader { get; set; }


        public string Hostname { get; set; }


    }
}
