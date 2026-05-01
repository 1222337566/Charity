using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Messages
{
    public class Balance : BaseEntity
    {
        public Balance() { }
        public int balance { get; set; }
    }
}
