using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;


[ApiController]
[Route("admin/ad")]
public class AdDirectoryController : ControllerBase
{
    private readonly ILdapDirectoryService _ldap;
    private readonly AppDbContext _db;
    private readonly IAdDirectorySyncService _sync;
    private readonly IDirectoryUserRepository _repo;
    private readonly IAdPromoteService _promote;
    private readonly IDirectoryDeviceRepository _deviceRepo;
    private readonly INetworkDeviceService _netsvc;
    public AdDirectoryController(
        ILdapDirectoryService ldap,
        IAdDirectorySyncService sync,
        IDirectoryUserRepository repo,
        IAdPromoteService promote,
        IDirectoryDeviceRepository device,
        INetworkDeviceService work,
        AppDbContext db)
    {
        _ldap = ldap; _sync = sync; _repo = repo; _promote = promote;
        _deviceRepo = device;
        _netsvc = work;
        _db = db;

    }

    // يعمل Discover من DHCP ويخزّن (بدون ARP)
    

    // يعمل Discover من DHCP ويخزّن (بدون ARP)
    [HttpGet("devices/{id:int}")]
    public async Task<IActionResult> GetDevices(int id, CancellationToken ct)
    {
        var user = await _db.DirectoryUsers.FindAsync(new object?[] { id }, ct);
        if (user is null) return NotFound(new { message = "DirectoryUser not found" });

        var list = await _deviceRepo.GetByUserIdAsync(id, ct);
        return Ok(list.Select(d => new {
            d.HostName,
            d.IpAddress,
            d.MacAddress,
            d.Source,
            d.FetchedAtUtc
        }));
    }
    [HttpGet("devices/refresh/{id:int}")]
    public async Task<IActionResult> RefreshDevices(int id, CancellationToken ct)
    {
        var user = await _db.DirectoryUsers.FindAsync(new object?[] { id }, ct);
        if (user is null) return NotFound(new { message = "DirectoryUser not found" });

        if (string.IsNullOrWhiteSpace(user.UserWorkstations))
            return BadRequest(new { message = "User has no userWorkstations set" });

        var updated = await _netsvc.DiscoverDevicesFromDhcpAsync(id, user.UserWorkstations, ct);
        return Ok(new
        {
            updated = updated.Count,
            items = updated.Select(d => new { d.HostName, d.IpAddress, d.MacAddress })
        });
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping(CancellationToken ct)
        => Ok(new { ok = await _ldap.PingAsync(ct) });

    // يسحب N مستخدم من LDAP ويحفظهم في DirectoryUser (refresh)
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromQuery] string? q, [FromQuery] string? ou, [FromQuery] int take = 100, CancellationToken ct = default)
    {
        var n = await _sync.RefreshAsync(q, ou, take, ct);
        return Ok(new { fetched = n });
    }
  
    // استعراض جدول DirectoryUser
    [HttpGet("directory")]
    public async Task<IActionResult> DirectoryList([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        var skip = (page - 1) * take;
        var total = await _repo.CountAsync(q, ct);
        var items = await _repo.SearchAsync(q, skip, take, ct);
        return Ok(new { total, page, take, items });
    }

    public record PromoteReq(string[] adObjectIds);

    // ترويج المحدد إلى Identity
    [HttpPost("promote")]
    public async Task<IActionResult> Promote([FromBody] PromoteReq req, CancellationToken ct = default)
    {
        if (req.adObjectIds is null || req.adObjectIds.Length == 0)
            return BadRequest(new { message = "No selection" });

        var n = await _promote.PromoteAsync(req.adObjectIds, ct);
        return Ok(new { affected = n });
    }
    [HttpGet("directory/export")]
    public async Task<IActionResult> ExportCsv([FromQuery] string? q, CancellationToken ct)
    {
        var all = await _repo.SearchAsync(q, 0, int.MaxValue, ct);
        var sb = new StringBuilder();
        sb.AppendLine("UPN,DisplayName,Department,UserWorkstations");
        foreach (var x in all)
        {
            sb.AppendLine($"\"{x.Upn}\",\"{x.DisplayName}\",\"{x.Department}\",\"{x.UserWorkstations}\"");
        }
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "directory_users.csv");
    }
}
