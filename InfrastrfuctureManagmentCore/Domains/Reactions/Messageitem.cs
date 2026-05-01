using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfrastructureManagmentCore.Domains.Reactions
{
    public class Messageitem
    {
        public string senderUsername { get; set; }
        public string senderid { get; set; }
        public string receiverid { get; set;}
        public string text { get; set; }
        public string receiverUsername { get; set; }
        public string time { get; set; }
    }
}