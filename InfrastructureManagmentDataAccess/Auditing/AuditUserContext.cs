using System.Threading;

namespace InfrastructureManagmentDataAccess.Auditing
{
    /// <summary>
    /// Holds the current request user for EF Core audit columns without changing AppDbContext constructor signature.
    /// </summary>
    public static class AuditUserContext
    {
        private static readonly AsyncLocal<AuditUserInfo?> Current = new();

        public static string? UserId => Current.Value?.UserId;
        public static string? UserName => Current.Value?.UserName;

        public static void Set(string? userId, string? userName)
        {
            Current.Value = new AuditUserInfo(userId, userName);
        }

        public static void Clear()
        {
            Current.Value = null;
        }

        private sealed record AuditUserInfo(string? UserId, string? UserName);
    }
}
