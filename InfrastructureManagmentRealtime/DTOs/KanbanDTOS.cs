namespace InfrastructureManagmentRealtime.DTOs
{
    public record MoveTaskDto(Guid TaskId, string NewStatus, int? OrderIndex);
    public sealed class CreateTaskDto
    {
        public Guid BoardId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? AssignedToUserId { get; set; }
        public DateTime? DueDateUtc { get; set; }
    }

    // Application/DTOs/Kanban/CreateBoardDto.cs
    public sealed class CreateBoardDto
    {
        public string Name { get; set; } = default!;
    }

    public sealed class UpdateTaskDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? AssignedToUserId { get; set; }
        public DateTime? DueDateUtc { get; set; }
    }
}
