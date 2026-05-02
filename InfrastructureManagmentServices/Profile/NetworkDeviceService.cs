using InfrastrfuctureManagmentCore.Domains.supplies;
using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using InfrastructureManagmentWebFramework.DTOs.Register;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public class NetworkDeviceService : INetworkDeviceService
    {
        private readonly IConfiguration _cfg;
        private readonly IDirectoryDeviceRepository _deviceRepo;

        public NetworkDeviceService(IConfiguration cfg, IDirectoryDeviceRepository deviceRepo)
        {
            _cfg = cfg; _deviceRepo = deviceRepo;
        }

        public async Task<List<DirectoryDevice>> DiscoverDevicesFromDhcpAsync(int userId, string userWorkstations, CancellationToken ct = default)
        {
            var server = _cfg["DHCP:Server"];
            if (string.IsNullOrWhiteSpace(server))
                throw new InvalidOperationException("DHCP:Server is not configured.");

            // أسماء الأجهزة من userWorkstations
            var names = userWorkstations.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .Where(s => !string.IsNullOrWhiteSpace(s))
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToArray();

            if (names.Length == 0) return new List<DirectoryDevice>();

            // استعلام DHCP عبر PowerShell
            var leases = await GetDhcpLeasesByHostnamesAsync(server, names, ct);

            // مابينج إلى DirectoryDevice
            var devices = leases.Select(l => new DirectoryDevice
            {
                DirectoryUserId = userId,
                HostName = l.HostName.ToUpperInvariant(),
                IpAddress = l.IpAddress,
                MacAddress = NormalizeMac(l.MacAddress),
                Source = "DHCP",
                FetchedAtUtc = DateTime.UtcNow
            }).ToList();

            await _deviceRepo.UpsertRangeAsync(devices, ct);
            return devices;
        }

        private static string NormalizeMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac)) return mac;
            // DHCP ClientId غالبًا "XX-XX-XX-XX-XX-XX" أو "XXXXXXXXXXXX"
            var hex = new string(mac.Where(c => "0123456789abcdefABCDEF".Contains(c)).ToArray());
            if (hex.Length != 12) return mac;
            return string.Join("-", Enumerable.Range(0, 6).Select(i => hex.Substring(i * 2, 2))).ToUpperInvariant();
        }
        private static PSCredential MakeCred(string domainUser, string password)
        {
            var secure = new SecureString();
            foreach (var c in password) secure.AppendChar(c);
            // domainUser = "SCWW2\\my.admin" أو UPN "my.admin@scww2.com.eg"
            return new PSCredential(domainUser, secure);
        }
        private static Task<List<DhcpLeaseInfo>> GetDhcpLeasesByHostnamesAsync(string dhcpServer, string[] hostnames, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dhcpServer))
                throw new ArgumentException("dhcpServer is null or empty");

            dhcpServer = dhcpServer.Trim();

            string host = dhcpServer; // << ركّز هنا: بنوصل مباشرة لسيرفر الـ DHCP
            var creds = MakeCred(@"admini@scww2.com.eg", "!@FGH@#$%^jkl2029#*");

            var shell = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            var uri = new Uri($"http://{host}:5985/wsman");

            var ci = new WSManConnectionInfo(uri, shell, creds)
            {
                OperationTimeout = 240000,
                OpenTimeout = 30000,
                AuthenticationMechanism = AuthenticationMechanism.Kerberos
            };

            return Task.Run(() =>
            {
                using var rs = RunspaceFactory.CreateRunspace(ci);
                rs.Open();

                using var ps = PowerShell.Create();
                ps.Runspace = rs;

                var script = @"
param($server, $names)

Import-Module DhcpServer -ErrorAction Stop | Out-Null

# احصل على كل الـ Scope IDs
$scopes = Get-DhcpServerv4Scope -ComputerName $server -ErrorAction Stop
$allLeases = @()
foreach ($scope in $scopes) {
    try {
        $allLeases += Get-DhcpServerv4Lease -ComputerName $server -ScopeId $scope.ScopeId -ErrorAction Stop
    } catch {
        Write-Warning ""فشل سكوب $($scope.ScopeId): $($_.Exception.Message)""
    }
}

# تأكد إن names مصفوفة
if (-not $names) { $names = @() }

function Normalize-Host([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return @() }
    $t = $s.Trim().TrimEnd('.').ToLowerInvariant()
    $short = ($t -split '\.')[0]
    if ($short -ne $t) { return @($t, $short) } else { return @($t) }
}

# استخدم Hashtable (case-insensitive by default)
$set = @{}
foreach ($n in $names) {
    foreach ($v in (Normalize-Host $n)) {
        if ($v -and -not $set.ContainsKey($v)) { $set[$v] = $true }
    }
}

if ($set.Count -eq 0) { return @() }

$filtered = @()
foreach ($lease in $allLeases) {
    if (-not $lease.HostName) { continue }
    $cands = Normalize-Host $lease.HostName
    foreach ($c in $cands) {
        if ($set.ContainsKey($c)) {
            $filtered += [PSCustomObject]@{
                HostName   = $lease.HostName
                IpAddress  = $lease.IPAddress.IPAddressToString
                MacAddress = $lease.ClientId
            }
            break
        }
    }
}

$filtered
";

                ps.AddScript(script);
                ps.AddParameter("names", hostnames ?? Array.Empty<string>());
                ps.AddParameter("server", dhcpServer);
                var results = ps.Invoke();
                if (ps.HadErrors)
                    throw new InvalidOperationException("PowerShell DHCP query failed: " + string.Join(Environment.NewLine, ps.Streams.Error.Select(e => e.ToString())));

                var list = new List<DhcpLeaseInfo>();
                foreach (var r in results)
                {
                    var h = r.Members["HostName"]?.Value?.ToString() ?? "";
                    var ip = r.Members["IpAddress"]?.Value?.ToString() ?? "";
                    var mac = r.Members["MacAddress"]?.Value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(h)) list.Add(new DhcpLeaseInfo(h, ip, mac));
                }
                return list;
            }, ct);
        }
    }
}
