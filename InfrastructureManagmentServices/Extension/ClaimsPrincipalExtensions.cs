using System.Security.Claims;

namespace InfrastructureManagmentInfrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// يحصل على الـUser Id من التوكن أو من Claims Identity.
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // أولاً نحاول نقرأ الـNameIdentifier الافتراضي
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // بعض الأنظمة تستخدم Claim مخصص مثل "sub"
            if (string.IsNullOrEmpty(id))
                id = user.FindFirst("sub")?.Value;

            // في حالة JWT مخصص (زي اللي في مشروعك، بيحتوي على Guid)
            if (string.IsNullOrEmpty(id))
                id = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(id))
                throw new InvalidOperationException("User ID not found in claims.");

            return id;
        }
    }
}