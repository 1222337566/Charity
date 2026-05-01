//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Reactions
{
    public class ChatMessage
    {
        [Key]
        [Column("Message ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Message_ID { get; set; }

        public int? senderid { get; set; }

        public int? Chatid { get; set; }

        public int? SPersonalid { get; set; }
        
        public string text { get; set; }

        public string time { get; set; }

        public int? receiver_id { get; set; }

        [ForeignKey("receiver_id")]
        public virtual Employee receiver { get; set; }
        [ForeignKey("senderid")]
        public virtual Employee sender { get; set; }
        
        [ForeignKey("SPersonalid")]
        public virtual PersonalInformation Spersonal { get; set; }
        public int? RPersonalid { get; set; }
        [ForeignKey("RPersonalid")]
        public virtual PersonalInformation Rpersonal { get; set; }
        [ForeignKey("Chatid")]
        public virtual Chat chat { get; set; }
    }
}