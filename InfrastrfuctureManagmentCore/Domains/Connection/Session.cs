using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Connection
{
    public class Session :BaseEntity
    {
        public Session() { 
           
        
        }

        public DateTime initialBeginTime { get; set; }

        public TimeSpan ExpirationPeriod { get; set; }

        public string TokenString { get; set; }

        public long? connectionID { get; set; }

        [ForeignKey("connectionID")]
        public virtual connection connection { get; set; } 
    }
}
