using InfrastructureManagmentServices.Search;
using InfrastructureManagmentWebFramework.DTOs.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Skote.Web.Api;

[ApiController]
[Route("api/[controller]")]
// ensure JWT
public class SearchController : ControllerBase
{
    private readonly ISearchService _svc;

    public SearchController(ISearchService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string q, [FromQuery] int take = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            return Ok(new { users = Array.Empty<object>(), devices = Array.Empty<object>(), dhcp = Array.Empty<object>(), meta = new { elapsedMs = 0 } });

        take = Math.Clamp(take, 3, 20);

        var sw = Stopwatch.StartNew();
        var result = await _svc.SearchAsync(q.Trim(), take, ct);
        sw.Stop();

        return Ok(new 
        {
            users = result.Users,
            devices = result.Devices,
            dhcp = result.Dhcp,
            meta = new { elapsedMs = sw.ElapsedMilliseconds }
        });
    }
}