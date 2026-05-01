using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsApiController : ControllerBase
    {
        private readonly INotificationRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsApiController(
            INotificationRepository repo,
            UserManager<ApplicationUser> userManager)
        {
            _repo        = repo;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> List(
            int skip = 0, int take = 20,
            string? level = null, bool? unread = null,
            CancellationToken ct = default)
        {
            var userId = _userManager.GetUserId(User)!;

            // unread=true → read=false; unread=false → read=true; unread=null → read=null
            bool? readFilter = unread.HasValue ? !unread.Value : null;

            var result = await _repo.QueryUserNotificationsAsync(
                userId, skip, take, level, kind: null, read: readFilter, ct);

            // count unread: read=false means unread
            var unreadResult = await _repo.QueryUserNotificationsAsync(
                userId, 0, 1000, null, null, read: false, ct);
            int unreadCount = unreadResult.Items.Count();

            return Ok(new {
                items = result.Items.Select(x => new {
                    id        = x.DeliveryId,
                    title     = x.Title,
                    message   = x.Message,
                    level     = x.Level,
                    kind      = x.Kind,
                    url       = x.Url,
                    isRead    = x.IsRead,
                    createdAt = x.CreatedAtUtc
                }),
                hasMore = result.HasMore,
                total   = skip + result.Items.Count() + (result.HasMore ? 1 : 0),
                unread  = unreadCount
            });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            await _repo.MarkDeliveryReadAsync(id, userId, ct);
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead(CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            await _repo.MarkAllReadAsync(userId, ct);
            return Ok();
        }
    }
}
