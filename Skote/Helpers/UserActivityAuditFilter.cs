using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Security.Claims;

namespace Skote.Helpers;

public sealed class UserActivityAuditFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> TrackedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private static readonly HashSet<string> IgnoredControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Account",
        "Auth",
        "UserActivityLogs"
    };

    private static readonly HashSet<string> InterestingKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "beneficiaryId", "donationId", "donorId", "funderId", "grantAgreementId", "grantInstallmentId",
        "projectId", "phaseId", "activityId", "taskId", "aidCycleId", "aidRequestId", "disbursementId", "invoiceId", "decisionId", "decision", "status", "approvalStatus", "executionStatus", "amount", "referenceNumber", "invoiceNumber", "requestNumber", "voucherNumber", "receiptNumber", "expenseNumber"
    };

    private readonly IUserActivityService _activityService;

    public UserActivityAuditFilter(IUserActivityService activityService)
    {
        _activityService = activityService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (ShouldSkip(context, executedContext))
        {
            return;
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? context.HttpContext.User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        var controller = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
        var actionName = context.RouteData.Values["action"]?.ToString() ?? string.Empty;
        var normalizedAction = NormalizeAction(actionName, context.HttpContext.Request.Method);
        var description = BuildDescription(context, controller, actionName);
        var entityName = controller;
        var entityId = ResolveEntityId(context);
        var newValues = CollectAuditValues(context);

        try
        {
            await _activityService.LogAsync(userId, normalizedAction, description, entityName, entityId, newValues: newValues, ct: context.HttpContext.RequestAborted);
        }
        catch
        {
            // لا نسمح لأي مشكلة في سجل الحركات أن تكسر التنفيذ الأساسي.
        }
    }

    private static bool ShouldSkip(ActionExecutingContext context, ActionExecutedContext executedContext)
    {
        var httpContext = context.HttpContext;
        if (httpContext.User?.Identity?.IsAuthenticated != true)
        {
            return true;
        }

        var method = httpContext.Request.Method;
        if (!TrackedMethods.Contains(method))
        {
            return true;
        }

        var controller = context.RouteData.Values["controller"]?.ToString();
        if (!string.IsNullOrWhiteSpace(controller) && IgnoredControllers.Contains(controller))
        {
            return true;
        }

        if (executedContext.Exception is not null && !executedContext.ExceptionHandled)
        {
            return true;
        }

        if (executedContext.Controller is Controller mvcController && !mvcController.ModelState.IsValid)
        {
            return true;
        }

        var statusCode = httpContext.Response?.StatusCode ?? StatusCodes.Status200OK;
        return statusCode >= StatusCodes.Status400BadRequest;
    }

    private static string NormalizeAction(string actionName, string method)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return method.ToUpperInvariant();
        }

        return actionName.Trim() switch
        {
            var x when x.Equals("Create", StringComparison.OrdinalIgnoreCase) => "Create",
            var x when x.Equals("Add", StringComparison.OrdinalIgnoreCase) => "Create",
            var x when x.Equals("Register", StringComparison.OrdinalIgnoreCase) => "Create",
            var x when x.Equals("Edit", StringComparison.OrdinalIgnoreCase) => "Update",
            var x when x.Equals("Update", StringComparison.OrdinalIgnoreCase) => "Update",
            var x when x.Equals("Delete", StringComparison.OrdinalIgnoreCase) => "Delete",
            var x when x.Equals("Remove", StringComparison.OrdinalIgnoreCase) => "Delete",
            var x when x.Equals("Review", StringComparison.OrdinalIgnoreCase) => "Review",
            var x when x.Equals("Approve", StringComparison.OrdinalIgnoreCase) => "Approve",
            var x when x.Equals("Reject", StringComparison.OrdinalIgnoreCase) => "Reject",
            var x when x.Equals("SubmitForReview", StringComparison.OrdinalIgnoreCase) => "Submit",
            var x when x.Equals("SendToCommittee", StringComparison.OrdinalIgnoreCase) => "Submit",
            var x when x.Equals("Committee", StringComparison.OrdinalIgnoreCase) => "Committee",
            var x when x.Equals("Disburse", StringComparison.OrdinalIgnoreCase) => "Disburse",
            var x when x.Equals("BatchDisburse", StringComparison.OrdinalIgnoreCase) => "BatchDisburse",
            var x when x.Equals("GenerateCycle", StringComparison.OrdinalIgnoreCase) => "GenerateCycle",
            var x when x.Equals("Generate", StringComparison.OrdinalIgnoreCase) => "Generate",
            var x when x.Equals("Assign", StringComparison.OrdinalIgnoreCase) => "Assign",
            var x when x.Equals("Unassign", StringComparison.OrdinalIgnoreCase) => "Unassign",
            var x when x.Equals("Import", StringComparison.OrdinalIgnoreCase) => "Import",
            var x when x.Equals("Upload", StringComparison.OrdinalIgnoreCase) => "Upload",
            var x when x.Equals("SeedDefaults", StringComparison.OrdinalIgnoreCase) => "SeedDefaults",
            var x when x.Equals("ResetAndSeedNow", StringComparison.OrdinalIgnoreCase) => "ResetAndSeed",
            _ => actionName.Length > 32 ? actionName[..32] : actionName
        };
    }

    private static string BuildDescription(ActionExecutingContext context, string controller, string actionName)
    {
        var parts = new List<string>
        {
            $"{controller}/{actionName}",
            $"Path={context.HttpContext.Request.Path}"
        };

        var keyValues = new List<string>();

        foreach (var route in context.RouteData.Values)
        {
            if (route.Key.Equals("controller", StringComparison.OrdinalIgnoreCase) ||
                route.Key.Equals("action", StringComparison.OrdinalIgnoreCase) ||
                route.Value is null)
            {
                continue;
            }

            keyValues.Add($"{route.Key}={route.Value}");
        }

        foreach (var argument in context.ActionArguments)
        {
            CollectInterestingValues(keyValues, argument.Key, argument.Value);
        }

        if (keyValues.Count > 0)
        {
            parts.Add(string.Join(" | ", keyValues.Distinct(StringComparer.OrdinalIgnoreCase)));
        }

        return string.Join(" | ", parts);
    }

    private static void CollectInterestingValues(List<string> bucket, string key, object? value)
    {
        if (value is null)
        {
            return;
        }

        if (TryFormatScalar(key, value, out var direct))
        {
            bucket.Add(direct);
            return;
        }

        var type = value.GetType();
        if (type == typeof(string) || type.IsPrimitive || type.IsEnum)
        {
            return;
        }

        foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var propertyValue = property.GetValue(value);
            if (TryFormatScalar(property.Name, propertyValue, out var nested))
            {
                bucket.Add(nested);
            }
        }
    }

    private static bool TryFormatScalar(string key, object? value, out string formatted)
    {
        formatted = string.Empty;

        if (value is null || (!InterestingKeys.Contains(key) && !key.EndsWith("Id", StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        string? text = null;

        if (value is Guid guid && guid != Guid.Empty)
        {
            text = guid.ToString();
        }
        else if (value is string s && !string.IsNullOrWhiteSpace(s) && s.Length <= 120)
        {
            text = s;
        }
        else if (value is int i)
        {
            text = i.ToString(CultureInfo.InvariantCulture);
        }
        else if (value is long l)
        {
            text = l.ToString(CultureInfo.InvariantCulture);
        }
        else if (value is decimal d)
        {
            text = d.ToString("0.##", CultureInfo.InvariantCulture);
        }
        else if (value is bool b)
        {
            text = b ? "true" : "false";
        }
        else if (value is DateTime dt)
        {
            text = dt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }
        else if (value is Enum e)
        {
            text = e.ToString();
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        formatted = $"{key}={text}";
        return true;
    }

    private static string? ResolveEntityId(ActionExecutingContext context)
    {
        var values = CollectAuditValues(context);
        foreach (var key in new[] { "id", "Id", "beneficiaryId", "BeneficiaryId", "aidRequestId", "AidRequestId", "invoiceId", "InvoiceId", "decisionId", "DecisionId", "requestId", "RequestId" })
        {
            if (values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return values.FirstOrDefault(x => x.Key.EndsWith("Id", StringComparison.OrdinalIgnoreCase)).Value;
    }

    private static Dictionary<string, string?> CollectAuditValues(ActionExecutingContext context)
    {
        var keyValues = new List<string>();

        foreach (var route in context.RouteData.Values)
        {
            if (route.Key.Equals("controller", StringComparison.OrdinalIgnoreCase) ||
                route.Key.Equals("action", StringComparison.OrdinalIgnoreCase) ||
                route.Value is null)
            {
                continue;
            }

            keyValues.Add($"{route.Key}={route.Value}");
        }

        foreach (var argument in context.ActionArguments)
        {
            CollectInterestingValues(keyValues, argument.Key, argument.Value);
        }

        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in keyValues.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var index = item.IndexOf('=');
            if (index <= 0)
            {
                continue;
            }

            var key = item[..index];
            var value = item[(index + 1)..];
            result[key] = value;
        }

        return result;
    }
}
