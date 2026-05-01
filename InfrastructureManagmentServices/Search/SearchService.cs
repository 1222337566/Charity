using InfrastructureManagmentServices.Search;
using InfrastructureManagmentWebFramework.DTOs.Search;
using Microsoft.Extensions.Logging;
using System.Formats.Tar;

public class SearchService : ISearchService
{
    private readonly IAdReader _ad;
    private readonly IDeviceRepo _devices;
    private readonly IDhcpReader _dhcp;
    private readonly ILogger<SearchService> _log;

    public SearchService(IAdReader ad, IDeviceRepo devices, IDhcpReader dhcp, ILogger<SearchService> log)
    {
        _ad = ad;
        _devices = devices;
        _dhcp = dhcp;
        _log = log;
    }

    public async Task<SearchResult> SearchAsync(string query, int take, CancellationToken ct)
    {
        // parallel fan-out
        var usersTask = _ad.FindUsersAsync(query, take, ct);
        var devicesTask = _devices.FindAsync(query, take, ct);
        var dhcpTask = _dhcp.FindAsync(query, take, ct);

        await Task.WhenAll(usersTask, devicesTask, dhcpTask);

        return new SearchResult
        {
            Users = usersTask.Result.Select(u => new { id = u.Id, name = u.Name, email = u.Email, ou = u.OU }),
            Devices = devicesTask.Result.Select(d => new { id = d.Id, name = d.Name, ip = d.IP, mac = d.MAC }),
            Dhcp = dhcpTask.Result.Select(h => new { deviceId = h.DeviceId, ip = h.IP, scope = h.Scope, state = h.State })
        };
    }
}