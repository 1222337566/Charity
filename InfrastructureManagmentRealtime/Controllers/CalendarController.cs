using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InfrastructureManagmentCore.Services.Calendar;
using InfrastructureManagmentRealtime.DTOs;
using InfrastructureManagmentCore.Services.Notifications;

namespace InfrastructureManagementRealtime.Controllers;

[ApiController]
[Route("calendar")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _cal;
    private readonly INotificationService _notificationservice;
    public CalendarController(ICalendarService cal,INotificationService notificationService)
    {
        _cal = cal;
        _notificationservice = notificationService;
    }

    string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

  
    // GET /calendar/events
    [HttpGet("events")]
    public async Task<IActionResult> Events([FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();
        // رجّع أحداث تخص المستخدم (مالك/مدعو)
        var items = await _cal.QueryAsync(uid, start, end, ct);
        return Ok(items.Select(e => new {
            id = e.Id,
            title = e.Title,
            startUtc = e.StartUtc,
            endUtc = e.EndUtc,
            allDay = e.AllDay,
            color = e.Color
        }));
    }

    // POST /calendar/create (يرسل دعوات للحاضرين)
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();
        var evId = await _cal.CreateAsync(uid, dto, ct);
       // var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();

        // … احفظ الحدث وارجع Id
         // مثال

        // ابعت دعوات للمشاركين
        foreach (var attendeeId in dto.AttendeesUserIds)
            await _notificationservice.SendCalendarInviteAsync(attendeeId, evId, dto.Title, dto.StartUtc, ct);

        return Ok(new { ok = true, eventId = evId });
    }

    // POST /calendar/respond (المشارك يرد)
    [HttpPost("respond")]
    public async Task<IActionResult> Respond([FromBody] RespondDto dto, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();

        // … حدّث حالة حضور المستخدم للحدث dto.EventId => dto.Status

        // ابعت إشعار لصاحب الحدث
        var ownerId = /* جبها من الحدث */ uid;
        var title = /* عنوان الحدث   */ "Event";
        await _notificationservice.SendCalendarRespondAsync(ownerId, dto.EventId, title, dto.Status, ct);

        return Ok(new { ok = true });
    }
    [Authorize]
    [HttpPost("respond1")]
    public async Task<IActionResult> Respond1([FromBody] CalendarRespondDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cal.RespondAsync1(dto.EventId, userId, dto.Status, ct);
        return Ok(new { ok = true });
    }
    // PUT /calendar/update
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateEventDto dto, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();
        await _cal.UpdateAsync(uid, dto, ct);
        return Ok(new { ok = true });
    }

    // DELETE /calendar/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid is null) return Unauthorized();
        await _cal.DeleteAsync(uid, id, ct);
        return Ok(new { ok = true });
    }


    // أحداث تخص المستخدم الحالي (مالِك/مدعو)
    [HttpGet("my")]
    public async Task<IActionResult> My()
    {
        if (UserId is null) return Unauthorized();
        var events = await _cal.GetMyAsync(UserId);
        return Ok(events);
    }
    
}
