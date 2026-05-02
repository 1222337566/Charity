using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Notify
{
    
        public class NotificationMessage
        {
            public string ToUserId { get; set; } = default!;
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string Level { get; set; } = "info";
            public string? Url { get; set; }
        }
    
}
