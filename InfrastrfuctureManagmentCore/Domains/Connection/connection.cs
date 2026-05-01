using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Connection
{
    public class connection : BaseEntity
    {
        public connection() {
            Sesstions = new HashSet<Session>();
        } 
        public string  Servername { get; set; }


        public string UserName { get; set; }
        public string UserPassword { get; set; }


        public virtual ICollection<Session> Sesstions { get; set; }


    }
}
