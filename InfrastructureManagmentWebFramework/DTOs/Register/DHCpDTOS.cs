using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Register
{
    public record DhcpLeaseInfo(string HostName, string IpAddress, string MacAddress);
}
