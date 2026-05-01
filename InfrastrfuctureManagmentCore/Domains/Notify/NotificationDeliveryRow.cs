using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Notify
{
    public sealed class NotificationDeliveryRow
    {
        public Guid DeliveryId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Level { get; set; } = "info";
        public string? Url { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public bool IsRead { get; set; }
        public string? Kind { get; set; }
        public object? Meta { get; set; } // أو string MetaJson وتسوّي deserialize في السيرفس
    }
}
