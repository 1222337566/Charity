using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfrastructureManagmentCore.Services.Chat;

namespace InfrastructureManagementRealtime.Controllers;

[ApiController]
[Route("chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;
    public ChatController(IChatService chat) => _chat = chat;

    // تاريخ الرسائل لغرفة معيّنة
    [HttpGet("room/history")]
    public async Task<IActionResult> RoomHistory([FromQuery] string room, [FromQuery] int take = 50)
    {
        var list = await _chat.GetRoomHistoryAsync(room, take);
        return Ok(list.OrderBy(x => x.SentAtUtc));
    }



}
