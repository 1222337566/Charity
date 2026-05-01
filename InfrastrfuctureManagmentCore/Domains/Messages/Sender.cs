using InfrastructureManagmentCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Messages
{
    public class Sender :BaseEntity
    {
        public string activity { get; set; }
        public string Type { get; set; }
        public long? LimitID { get; set; }

        [ForeignKey("LimitID")]

        public virtual Limit Limt
        { get; set; }

        public long? balanceID { get; set; }

        [ForeignKey("balanceID")]

        public virtual Balance Balance
        { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public long? CatId { get; set; }
        [ForeignKey("CatId")]
        public virtual Category Category { get; set; }
    }
}
