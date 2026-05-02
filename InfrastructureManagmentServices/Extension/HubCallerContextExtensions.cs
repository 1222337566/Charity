using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace InfrastructureManagmentInfrastructure.Extensions
{
    public static class HubCallerContextExtensions
    {
        public static string GetUserId(this HubCallerContext ctx)
        {
            var user = ctx?.User ?? throw new InvalidOperationException("No user in Hub context.");
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                ?? throw new InvalidOperationException("User ID not found in claims.");
        }
    }
}