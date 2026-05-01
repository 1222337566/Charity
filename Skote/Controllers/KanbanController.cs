// Controllers/KanbanController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class KanbanController : Controller
{
    // GET: /Kanban/Boards
    [HttpGet]
    public IActionResult Boards()
    {
        // يعرض View: Views/Kanban/Boards.cshtml
        return View();
    }

    // GET: /Kanban/Index?board=<guid>
    [HttpGet]
    public IActionResult Index(Guid? board)
    {
        // مرّر BoardId للـView (لو null خليه string.Empty)
        var vm = new KanbanIndexVm { BoardId = board?.ToString() ?? string.Empty };
        return View(vm); // Views/Kanban/Index.cshtml
    }
    [HttpGet]
    public IActionResult Taskkanban(Guid? board)
    {
        // مرّر BoardId للـView (لو null خليه string.Empty)
        var vm = new KanbanIndexVm { BoardId = board?.ToString() ?? string.Empty };
        return View(vm); // Views/Kanban/Index.cshtml
    }
}

// ViewModel بسيط للصفحة
public class KanbanIndexVm
{
    public string BoardId { get; set; } = "";
}
