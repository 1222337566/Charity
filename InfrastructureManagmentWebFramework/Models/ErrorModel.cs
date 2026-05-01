using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models
{
    public partial  record ErrorModel : BaseModel
    {
        public ErrorModel() { } 

        
        public Int16 ErrorID { get; set; }

        public Int16 ErrorCode { get; set; }

        public string  Status { get; set; }


        public string ErrorMessage { get; set; }

    }
}
