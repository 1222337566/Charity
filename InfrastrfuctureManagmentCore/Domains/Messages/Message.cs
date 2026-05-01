//using BillingCore.Domains.Messages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Messages
{
    public class Message : BaseEntity
    {
        public string header_text { get; set; }
        public string body_text { get; set; }
        public Int64 messageId { get; set; }
        
        public long? SenderID { get; set; }

        public long? ReceiverId { get; set; }
        

        public Int64 AccountNumber { get; set; }

        public string CustomerName { get; set; }

        public DateTime SendTime { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
        [ForeignKey("ReceiverId")]

        public Receiver Receiver { get; set; }
        [ForeignKey("SenderID")]

        public virtual Sender Sender { get; set; }
        public long? CatId { get; set; }
        [ForeignKey("CatId")]
        public virtual Category Category { get; set; }
        
    }
}
