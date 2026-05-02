using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.supplies
{
    public class DhcpRecord
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; } = ""; // اختياري
        public string IP { get; set; } = "";
        public string Scope { get; set; } = "";
        public string State { get; set; } = ""; // Active/Reserved/...
        public string MAC { get; set; } = "";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
