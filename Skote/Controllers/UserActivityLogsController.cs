using InfrastrfuctureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
using Skote.ViewModels.Audit;
using System.Text;
using System.Text.Json;

namespace Skote.Controllers;

[Authorize(Policy = CharityPolicies.AuditView)]
public class UserActivityLogsController : Controller
{
    private readonly IUserActivityLogRepository _repo;

    public UserActivityLogsController(IUserActivityLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(string? query, string? action, string? userId, string? ipAddress, DateTime? fromUtc, DateTime? toUtc, int take = 200)
    {
        action = action == "Index" ? null : action;
        take = Math.Clamp(take <= 0 ? 200 : take, 20, 1000);

        var items = await _repo.SearchAsync(query, action, userId, ipAddress, fromUtc, toUtc, take);
        var summary = await _repo.GetSummaryAsync(fromUtc, toUtc);
        var actions = await _repo.GetActionsAsync();

        var vm = new UserActivityLogIndexVm
        {
            Query = query,
            Action = action,
            UserId = userId,
            IpAddress = ipAddress,
            FromUtc = fromUtc,
            ToUtc = toUtc,
            Take = take,
            AvailableActions = actions.ToList(),
            Summary = new AuditSummaryVm
            {
                TotalCount = summary.TotalCount,
                TodayCount = summary.TodayCount,
                Last7DaysCount = summary.Last7DaysCount,
                DistinctUsers = summary.DistinctUsers
            },
            Items = items.Select(x =>
            {
                var payload = ReadPayload(x.Name);
                return new UserActivityLogRowVm
                {
                    Id = x.Id ?? 0,
                    UserId = x.UserId ?? string.Empty,
                    UserName = payload.UserName ?? x.UserId ?? string.Empty,
                    Action = x.Action ?? string.Empty,
                    ControllerName = payload.ControllerName ?? string.Empty,
                    ActionName = payload.ActionName ?? string.Empty,
                    Description = payload.Description ?? string.Empty,
                    EntityName = payload.EntityName ?? string.Empty,
                    EntityId = payload.EntityId ?? string.Empty,
                    OccurredAtUtc = x.OccurredAtUtc,
                    IpAddress = x.IpAddress ?? string.Empty,
                    UserAgent = x.UserAgent ?? string.Empty
                };
            }).ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var payload = ReadPayload(item.Name);
        var vm = new UserActivityLogDetailsVm
        {
            Id = item.Id ?? 0,
            UserId = item.UserId ?? string.Empty,
            UserName = payload.UserName ?? item.UserId ?? string.Empty,
            Action = item.Action ?? string.Empty,
            ControllerName = payload.ControllerName ?? string.Empty,
            ActionName = payload.ActionName ?? string.Empty,
            HttpMethod = payload.HttpMethod ?? string.Empty,
            Path = payload.Path ?? string.Empty,
            Description = payload.Description ?? string.Empty,
            EntityName = payload.EntityName ?? string.Empty,
            EntityId = payload.EntityId ?? string.Empty,
            OldValues = payload.OldValues ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
            NewValues = payload.NewValues ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
            OccurredAtUtc = item.OccurredAtUtc,
            IpAddress = item.IpAddress ?? string.Empty,
            UserAgent = item.UserAgent ?? string.Empty
        };

        return View(vm);
    }

    public async Task<IActionResult> ExportCsv(string? query, string? action, string? userId, string? ipAddress, DateTime? fromUtc, DateTime? toUtc, int take = 1000)
    {
        take = Math.Clamp(take <= 0 ? 1000 : take, 50, 5000);
        var items = await _repo.SearchAsync(query, action, userId, ipAddress, fromUtc, toUtc, take);

        var sb = new StringBuilder();
        sb.AppendLine("Id,UserId,UserName,Action,Controller,ActionName,Description,EntityName,EntityId,OccurredAtUtc,IpAddress,UserAgent");

        foreach (var item in items)
        {
            var payload = ReadPayload(item.Name);
            static string Esc(string? value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
            sb.AppendLine(string.Join(',',
                item.Id ?? 0,
                Esc(item.UserId),
                Esc(payload.UserName),
                Esc(item.Action),
                Esc(payload.ControllerName),
                Esc(payload.ActionName),
                Esc(payload.Description),
                Esc(payload.EntityName),
                Esc(payload.EntityId),
                Esc(item.OccurredAtUtc.ToString("yyyy-MM-dd HH:mm:ss")),
                Esc(item.IpAddress),
                Esc(item.UserAgent)));
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"user-activity-logs-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }

    private static AuditPayloadVm ReadPayload(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new AuditPayloadVm();
        }

        try
        {
            var payload = JsonSerializer.Deserialize<AuditPayloadVm>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return payload ?? new AuditPayloadVm();
        }
        catch
        {
            return new AuditPayloadVm { Description = json };
        }
    }

    private sealed class AuditPayloadVm
    {
        public string? UserName { get; set; }
        public string? Description { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? HttpMethod { get; set; }
        public string? Path { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public Dictionary<string, string?>? OldValues { get; set; }
        public Dictionary<string, string?>? NewValues { get; set; }
    }
}
