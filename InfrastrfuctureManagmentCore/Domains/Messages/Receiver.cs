using InfrastructureManagmentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Messages
{
    public class Receiver :BaseEntity
    {
        public string Telephone { get; set; }

        public long telephonenum
        {  get; set; }
            //=> long.Parse(Telephone.Split('0')[1]);
        //public BigInteger telephonenum => BigInteger.Parse(Telephone);
        public string Type { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
