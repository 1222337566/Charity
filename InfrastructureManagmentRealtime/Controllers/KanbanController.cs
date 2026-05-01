using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastructureManagmentCore.Domains.Connection;
using InfrastructureManagmentInfrastructure.Extensions;
using InfrastructureManagmentRealtime.DTOs;
using InfrastructureManagmentServices.Kanban;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("kanban")]
[Authorize]
public class KanbanController : ControllerBase
{
    private readonly IKanbanService _svc;
    private readonly ITaskBoardRepository _boards;
    public KanbanController(IKanbanService svc, ITaskBoardRepository boards) { _svc = svc;
        _boards = boards;
    }

    [HttpGet("boards/{boardId:guid}/tasks")]
    public async Task<IActionResult> GetTasks([FromRoute] Guid boardId)
       => Ok(await _svc.GetTasksAsync(boardId, User.GetUserId()));

    [HttpPost("tasks")]
    public async Task<IActionResult> Create([FromBody] KanbanTask dto)
    => Ok(await _svc.CreateTaskAsync(dto, User.GetUserId()));

    [HttpPost("tasks/move")]
    public async Task<IActionResult> Move([FromBody] MoveTaskDto dto)
    {
        await _svc.MoveTaskAsync(dto.TaskId, dto.NewStatus, dto.OrderIndex, User.GetUserId());
        return Ok(new { ok = true });
    }

    [HttpPut("tasks")]
    public async Task<IActionResult> Update([FromBody] KanbanTask dto)
    {
        await _svc.UpdateTaskAsync(dto, User.GetUserId());
        return Ok(new { ok = true });
    }

    [HttpDelete("tasks/{taskId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid taskId)
    {
        await _svc.DeleteTaskAsync(taskId, User.GetUserId());
        return Ok(new { ok = true });
    }
    // NEW: إنشاء Board
    [HttpPost("boards")]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardDto dto, CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var board = await _boards.CreateAsync(new TaskBoard
        {
            Name = dto.Name,
            CreatedByUserId = uid
        }, ct);

        return Ok(new { board.Id, board.Name });
    }

    // NEW: إرجاع الـBoards الخاصة بالمستخدم الحالي
    [HttpGet("boards/mine")]
    public Task<List<TaskBoard>> MyBoards(CancellationToken ct)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _boards.ListMineAsync(uid, ct);
    }

    // GET: /kanban/boards/{boardId}/members
    [HttpGet("boards/{boardId:guid}/members")]
    public async Task<IActionResult> Members([FromRoute] Guid boardId, CancellationToken ct)
    {
        var uid = User.GetUserId();
        var data = await _boards.GetMembersAsync(boardId, uid, ct);
        return Ok(data.Select(x => new { x.UserId, x.Role }));
    }

    public sealed class AddMemberDto { public string UserId { get; set; } = default!; public string Role { get; set; } = "Member"; }

    // POST: /kanban/boards/{boardId}/members
    [HttpPost("boards/{boardId:guid}/members")]
    public async Task<IActionResult> AddMember([FromRoute] Guid boardId, [FromBody] AddMemberDto dto, CancellationToken ct)
    {
        await _boards.AddMemberAsync(boardId, dto.UserId, dto.Role, User.GetUserId(), ct);
        return Ok(new { ok = true });
    }

    // DELETE: /kanban/boards/{boardId}/members/{userId}
    [HttpDelete("boards/{boardId:guid}/members/{userId}")]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid boardId, [FromRoute] string userId, CancellationToken ct)
    {
        await _boards.RemoveMemberAsync(boardId, userId, User.GetUserId(), ct);
        return Ok(new { ok = true });
    }


}
