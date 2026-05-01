using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using InfrastructureManagmentCore.Domains.Profiling;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using InfrastrfuctureManagmentCore.Domains.Progiling;
//using Microsoft.AspNet.Identity.EntityFrameworkCore;

namespace InfrastructureManagmentCore.Domains.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    //var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        ////    return userIdentity;
        //}

        public int? employeeid { get; set; }
     
        public int? profileid { get; set; }
        [ForeignKey("employeeid")]
        public virtual Employee employee { get; set; }
        [ForeignKey("profileid")]
        public virtual PersonalInformation personal { get; set; }
        public virtual ICollection<UserLog> UserLogs { get; set; }
        public string? DisplayName { get; set; }
        public string? Department { get; set; }
        public string? AdObjectId { get; set; }
        
        /// <summary>ربط مع موظف HR — يُملأ عند إنشاء حساب من ملف الموظف</summary>
        public Guid? HrEmployeeId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginUtc { get; set; }
       // public virtual DirectoryUser? DirectoryUser { get; set; } 

    }
    public class UserLog
    {
        [Key]
        public int UserLogID { get; set; }

        public string IPAD { get; set; }
        public DateTime LoginDate { get; set; }


        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
    public class Country
    {

        public int id { get; set; }

        public string iso { get; set; }
        public string name { get; set; }
        public string nicename { get; set; }
        public string iso3 { get; set; }
        public string numcode { get; set; }
        public string phonecode { get; set; }

    }
}
