using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public class DhcpWindowsReader : IDhcpReader
    {
        private readonly string _server;
        public DhcpWindowsReader(IConfiguration cfg) => _server = cfg.GetSection("Dhcp")["Server"] ?? "localhost";
        private static PSCredential MakeCred(string domainUser, string password)
        {
            var secure = new SecureString();
            foreach (var c in password) secure.AppendChar(c);
            // domainUser = "SCWW2\\my.admin" أو UPN "my.admin@scww2.com.eg"
            return new PSCredential(domainUser, secure);
        }
        public async Task<IEnumerable<(string DeviceId, string IP, string Scope, string State)>> FindAsync(string q, int take, CancellationToken ct)
        {
            var creds = MakeCred(@"admini@scww2.com.eg", "!@FGH@#$%^jkl2029#*");
            string host = "wdc01.scww2.com.eg";
            var shell = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            var uri = new Uri($"http://{host}:5985/wsman");

            var ci = new WSManConnectionInfo(uri, shell, creds)
            {
                OperationTimeout = 240000,
                OpenTimeout = 30000,
                AuthenticationMechanism = AuthenticationMechanism.Kerberos
            };
            return await Task.Run(() =>
            {
                using var rs = RunspaceFactory.CreateRunspace(ci);
                rs.Open();

                using var ps = PowerShell.Create();
                ps.Runspace = rs;
                //using var ps = PowerShell.Create();
                // بحث مبسّط في كل الـScopes ثم فلترة
                ps.AddScript($@"
$server = '{_server}';
$result = @()
Get-DhcpServerv4Scope -ComputerName $server | ForEach-Object {{
  $scope = $_.ScopeId
  Get-DhcpServerv4Lease -ComputerName $server -ScopeId $scope -ErrorAction SilentlyContinue | ForEach-Object {{
    [PSCustomObject]@{{
      IP = $_.IPAddress
      Scope = $scope.IPAddressToString
      State = $_.AddressState
      Mac = $_.ClientId
      Hostname = $_.HostName
    }}
  }}
}}
# فلترة بسيطة بالـq
if ('{q}'.Length -gt 0) {{
  $result = $result | Where-Object {{ $_.IP -like ""*{q}*"" -or $_.Scope -like ""*{q}*"" -or $_.Mac -like ""*{q}*"" -or $_.Hostname -like ""*{q}*"" }}
}} else {{
  $result = $result
}}
# Take
$result | Select-Object -First {take}
");
                var rows = ps.Invoke();
                var list = new List<(string DeviceId, string IP, string Scope, string State)>();
                foreach (var r in rows)
                {
                    var ip = r.Members["IP"]?.Value?.ToString() ?? "";
                    var scope = r.Members["Scope"]?.Value?.ToString() ?? "";
                    var state = r.Members["State"]?.Value?.ToString() ?? "";
                    list.Add(("", ip, scope, state));
                }
                return list.AsEnumerable();
            }, ct);
        }
    }
}
