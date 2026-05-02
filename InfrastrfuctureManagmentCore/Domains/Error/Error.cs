using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.NewFolder1
{
    public  class Error : BaseEntity
    {
        public Int16 ErrorCode { get; set; }
        public long? objID { get; set; }

        public int TaskID { get; set; }
        public string Status { get; set; }

        public DateTime DateTime { get; set; }
        public string ErrorMessage { get; set; }

    }
}
