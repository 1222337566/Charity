using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagementRealtime.Helpers;
using InfrastructureManagementRealtime.Hubs;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentCore.Services.Notifications;
using System.Security.Claims;
using InfrastructureManagementRealtime.DTOs;

namespace InfrastructureManagementRealtime.Controllers;

[ApiController]
[Route("notify")]
[Authorize] // ممكن تضيف Roles لو عايز: (Roles = "Admin")
public class NotifyController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly INotificationBus _bus;
    private readonly INotificationService _svc;

    public NotifyController(AppDbContext db, INotificationBus bus, INotificationService svc)
    {
        _db = db;
        _bus = bus;
        _svc = svc;
    }
    string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

   

    // ========== User by Name ==========
    [HttpPost("user/by-user")]
    public async Task<IActionResult> NotifyByUser([FromBody] NotifyUserDto dto)
    {
        var user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserName == dto.UserName);
        if (user is null) return NotFound(new { error = "User not found" });

        var p = new NotificationPayload(dto.Title, dto.Message, null, null, "info", DateTime.UtcNow.Ticks);
        await _bus.ToUserAsync(user.Id, p);
        return Ok(new { ok = true, userId = user.Id });
    }

    // إرسال لمستخدم معيّن (by userId)
    [HttpPost("user/by-id")]
    public async Task<IActionResult> ById([FromBody] ByIdDto dto)
    {
        if (UserId is null) return Unauthorized();
        var id = await _svc.ToUserAsync(UserId, dto.UserId, dto.Title, dto.Message);
        return Ok(new { ok = true, id });
    }

    // صندوق وارد المستخدم الحالي
    [HttpGet("my")]
    public async Task<IActionResult> My([FromQuery] bool unreadOnly = false, [FromQuery] int take = 50)
    {
        if (UserId is null) return Unauthorized();
        var list = await _svc.GetMyInboxAsync(UserId, unreadOnly, take);
        return Ok(list);
    }

    // تعليم إشعار كمقروء
    [HttpPost("read")]
    public async Task<IActionResult> Read([FromBody] Guid deliveryId)
    
    {
        if (UserId is null) return Unauthorized();
        var ok = await _svc.MarkReadAsync(deliveryId, UserId);
        return ok ? Ok(new { ok = true }) : NotFound();
    }

    // إرسال لموضوع Topic
    [HttpPost("topic")]
    public async Task<IActionResult> Topic([FromBody] TopicDto dto)
    {
        if (UserId is null) return Unauthorized();
        await _svc.ToTopicAsync(UserId, dto.Topic, dto.Title, dto.Message);
        return Ok(new { ok = true });
    }

    // إرسال عام (Broadcast)
    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastDto1 dto)
    {
        if (UserId is null) return Unauthorized();
        await _svc.ToAllAsync(UserId, dto.Title, dto.Message);
        return Ok(new { ok = true });
    }
    // ========== User by Id ==========
    //[HttpPost("user/by-id")]
    //public async Task<IActionResult> NotifyById([FromBody] NotifyUserIdDto dto)
    //{
    //    var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Id == dto.UserId);
    //    if (!exists) return NotFound(new { error = "User not found" });

    //    var p = new NotificationPayload(dto.Title, dto.Message, null, null, "info", DateTime.UtcNow.Ticks);
    //    await _bus.ToUserAsync(dto.UserId, p);
    //    return Ok(new { ok = true, userId = dto.UserId });
    //}

    //// ========== Topic ==========
    //[HttpPost("topic")]
    //public async Task<IActionResult> NotifyTopic([FromBody] NotifyTopicDto dto)
    //{
    //    var p = new NotificationPayload(dto.Title, dto.Message, null, null, "warning", DateTime.UtcNow.Ticks);
    //    await _bus.ToTopicAsync(dto.Topic, p);
    //    return Ok(new { ok = true, topic = dto.Topic });
    //}

    //// ========== Broadcast ==========
    //[HttpPost("broadcast")]
    //public async Task<IActionResult> NotifyBroadcast([FromBody] NotifyBroadcastDto dto)
    //{
    //    var p = new NotificationPayload(dto.Title, dto.Message, null, null, "info", DateTime.UtcNow.Ticks);
    //    await _bus.BroadcastAsync(p);
    //    return Ok(new { ok = true });
    //}
    [HttpPost("notify/read/all")]
    public async Task<IActionResult> ReadAll(CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();
        var count = await _svc.MarkAllReadAsync(uid, ct);
        return Ok(new { ok = true, count });
    }
    // ========== Test for current user ==========
    [HttpPost("test")]
    public async Task<IActionResult> TestForCaller()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        
        var p = new NotificationPayload("Test", "Hello from /notify/test", null, null, "info", DateTime.UtcNow.Ticks);
        await _bus.ToUserAsync(userId, p);
        return Ok(new { ok = true });
    }
}
