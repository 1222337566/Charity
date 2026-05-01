using InfrastructureManagmentCore.Domains.Complains;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentCore.Domains.Requests;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastructureManagmentCore.Domains.Documents;
using InfrastructureManagmentCore.Domains.Projects;
using Microsoft.AspNetCore.Http;
using InfrastructureManagmentCore.Domains.Identity;

namespace InfrastructureManagmentCore.Domains.Profiling
{
    public class PersonalInformation
    {
       
      [Key]
      [Column("person_id")]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int person_id { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        // Personal info
        public string? FullName { get; set; }
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string ProfileImagePath { get; set; }

      //  public int Id { get; set; }

        // ربط بالمستخدم (Identity)
        public string UserId { get; set; }
        
        public ApplicationUser User { get; set; }

        //public string ProfileImagePath { get; set; }
        // ملف الصورة (لو API/Razor هتعديه من الـ Controller)
        //public IFormFile ProfileImage { get; set; }
        [Column(TypeName = "text")]
            public string? title { get; set; }
            [Column("name of operation", TypeName = "text")]

            public string? description { get; set; }

            [Column(TypeName = "text")]
            public string? affectedobject { get; set; }

            public string? Type { get; set; }
            public string? Sender { get; set; }

            public string? Receiver { get; set; }

            public string? First_Name { get; set; }

            //public string Gender { get; set; }
            public int? Trial { get; set; }
            public int? Accounting { get; set; }
            public int? Finincial { get; set; }
            public int? System { get; set; }
            public int? Chat { get; set; }
            public int? Event { get; set; }
            public int? Graphs { get; set; }
            public int? Social { get; set; }
            public int? Project { get; set; }
            public int? CProject { get; set; }
            public int? Notification { get; set; }
            public int? Todo { get; set; }
            public int? Complain { get; set; }
            public int? TT { get; set; }
            //public DateTime BirthDate { get; set; }
            public string? Last_Name { get; set; }
            public Boolean? Read { get; set; }
            public DateTime? date { get; set; }
            public DateTime? EXdate { get; set; }
            public string? ContentType { get; set; }

            public byte[]? ContentData { get; set; }

            public int? GroupId { get; set; }
            [ForeignKey("GroupId")]
            public virtual Group Groups { get; set; }
            public FileType? FileType { get; set; }
            public int? Extension { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Complain> Complains { get; set; }
            public virtual ICollection<ChatMessage> SMessages { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<ChatMessage> RMessages { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Comment> Comments { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Notification> Notifications { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Post> Posts { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Project> Projects { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Request> Requests { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<RequestResponse> RequestResponses { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Todo> Todos { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TroubleTicket> TroubleTickets { get; set; }
        }

    }


