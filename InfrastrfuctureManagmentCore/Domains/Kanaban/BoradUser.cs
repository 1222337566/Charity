using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Kanaban
{
    public class BoardUser
    {
        public Guid BoardId { get; set; }
        public string UserId { get; set; } = default!;
        public string Role { get; set; } = "Member"; // Owner/Member/Viewer
                                                     // (اختياري ولكن يُفضّل)
        public TaskBoard? Board { get; set; }
    }
}
