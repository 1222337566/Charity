using InfrastrfuctureManagmentCore.Domains.Progiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.supplies
{
    public class DirectoryDevice
    {
        public int Id { get; set; }

        public int DirectoryUserId { get; set; }
        public DirectoryUser User { get; set; } = default!;

        public string HostName { get; set; } = default!;
        public string? IpAddress { get; set; }
        public string? MacAddress { get; set; }

        public string Source { get; set; } = "DNS"; // DNS / DHCP / ARP
        public DateTime FetchedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
