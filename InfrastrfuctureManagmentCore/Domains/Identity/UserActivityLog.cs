using InfrastructureManagmentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Identity
{
    public class UserActivityLog : BaseEntity
    {
        public string UserId { get; set; }     // FK -> ApplicationUser.Id
        public string Action { get; set; }     // "Login" | "Logout"
        public DateTime OccurredAtUtc { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }


    }
}
