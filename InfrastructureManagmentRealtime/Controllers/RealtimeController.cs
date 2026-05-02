using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/realtime")]
public class RealtimeController : ControllerBase
{
    private readonly IHubContext<NotificationsHub, INotificationsClient> _hub;
    public RealtimeController(IHubContext<NotificationsHub, INotificationsClient> hub) => _hub = hub;

    [HttpPost("broadcast")]
    [Authorize(Roles = "Admin")] // غيّر السياسة حسب احتياجك
    public async Task<IActionResult> Broadcast([FromBody] BroadcastDto dto)
    {
        await _hub.Clients.All.Receive(dto.Type, dto.Message, dto.Payload);
        return Ok();
    }

    [HttpPost("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> ToUser(string userId, [FromBody] BroadcastDto dto)
    {
        await _hub.Clients.Group($"user:{userId}").Receive(dto.Type, dto.Message, dto.Payload);
        return Ok();
    }

    [HttpPost("role/{role}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToRole(string role, [FromBody] BroadcastDto dto)
    {
        await _hub.Clients.Group($"role:{role}").Receive(dto.Type, dto.Message, dto.Payload);
        return Ok();
    }
}

public class BroadcastDto
{
    public string Type { get; set; }    // ex: "profile.updated"
    public string Message { get; set; } // ex: "Ahmed updated his profile"
    public object Payload { get; set; } // أي بيانات إضافية
}
