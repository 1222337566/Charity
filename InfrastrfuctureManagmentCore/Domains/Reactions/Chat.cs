//using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using System.Runtime.InteropServices;
//using WebGrease.Activities;
namespace InfrastructureManagmentCore.Domains.Reactions
{
    public class Chat
    {
        [Key]
        [Column("Chat ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int chatid { get; set; }

        public string starttime { get; set; }

        public string endtime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}