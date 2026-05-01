using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentServices.Kanban;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Skote.Controllers
{
    [Authorize]
    public class MyBoardController : Controller
    {
        private readonly ITaskBoardRepository _boards;
        private readonly IKanbanService _kanban;

        private readonly UserManager<ApplicationUser> _userManager;

        public MyBoardController(
            ITaskBoardRepository boards,
            IKanbanService kanban,
            UserManager<ApplicationUser> userManager)
        {
            _boards      = boards;
            _kanban      = kanban;
            _userManager = userManager;
        }

        // GET /MyBoard  — يعرض بورد المستخدم الشخصي أو يُنشئه تلقائياً
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            var user   = await _userManager.GetUserAsync(User);

            // جلب البورد الشخصي أو إنشاؤه
            var boards = await _boards.ListMineAsync(userId, ct);
            var board  = boards.FirstOrDefault(b => b.Name.Contains("الشخصي") || b.Name.Contains("My Board"))
                         ?? boards.FirstOrDefault();

            if (board == null)
            {
                board = await _boards.CreateAsync(new TaskBoard
                {
                    Name            = $"بوردي الشخصي — {user?.UserName ?? userId}",
                    CreatedByUserId = userId
                }, ct);
            }

            // جلب المهام
            var tasks = await _kanban.GetBoardAsync(board.Id, ct);

            ViewBag.BoardId   = board.Id;
            ViewBag.BoardName = board.Name;
            ViewBag.UserName  = user?.UserName ?? "المستخدم";

            var vm = new MyBoardVm
            {
                BoardId    = board.Id,
                BoardName  = board.Name,
                ToDo       = tasks.Where(t => t.Status == "ToDo").OrderBy(t => t.CreatedAtUtc).ToList(),
                InProgress = tasks.Where(t => t.Status == "InProgress").OrderBy(t => t.CreatedAtUtc).ToList(),
                Blocked    = tasks.Where(t => t.Status == "Blocked").OrderBy(t => t.CreatedAtUtc).ToList(),
                Done       = tasks.Where(t => t.Status == "Done").OrderByDescending(t => t.UpdatedAtUtc).Take(10).ToList(),
            };

            return View(vm);
        }

        // POST /MyBoard/AddTask
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(Guid boardId, string title,
            string priority, string? description, DateTime? dueDate,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest();

            var userId = _userManager.GetUserId(User)!;
            await _kanban.CreateAsync(boardId, title, description, priority ?? "Medium",
                userId, dueDate, userId, ct);

            return RedirectToAction(nameof(Index));
        }

        // POST /MyBoard/MoveTask
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveTask(Guid taskId, string newStatus,
            CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            await _kanban.MoveAsync(taskId, newStatus, userId, ct);
            // SignalR broadcast handled client-side via direct hub call
            return Ok(new { ok = true });
        }

        // POST /MyBoard/DeleteTask
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken ct)
        {
            await _kanban.DeleteAsync(taskId, ct);
            return RedirectToAction(nameof(Index));
        }
    }

    public class MyBoardVm
    {
        public Guid   BoardId   { get; set; }
        public string BoardName { get; set; } = "";
        public List<TaskItem> ToDo       { get; set; } = new();
        public List<TaskItem> InProgress { get; set; } = new();
        public List<TaskItem> Blocked    { get; set; } = new();
        public List<TaskItem> Done       { get; set; } = new();
        public int Total    => ToDo.Count + InProgress.Count + Blocked.Count + Done.Count;
        public int Overdue  => ToDo.Concat(InProgress).Concat(Blocked)
                                  .Count(t => t.DueDateUtc.HasValue && t.DueDateUtc.Value < DateTime.UtcNow);
    }
}
