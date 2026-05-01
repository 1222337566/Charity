using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Skote.Helpers;

namespace Skote.TagHelpers;

[HtmlTargetElement("authz", Attributes = "policy")]
[HtmlTargetElement("authz", Attributes = "any-policy")]
[HtmlTargetElement("authz", Attributes = "permission")]
public class AuthorizePolicyTagHelper : TagHelper
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizePolicyTagHelper(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HtmlAttributeName("policy")]
    public string? Policy { get; set; }

    [HtmlAttributeName("any-policy")]
    public string? AnyPolicy { get; set; }

    [HtmlAttributeName("permission")]
    public string? Permission { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = default!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var user = ViewContext?.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            output.SuppressOutput();
            return;
        }

        var isAuthorized = false;

        if (!string.IsNullOrWhiteSpace(Policy))
        {
            var result = await _authorizationService.AuthorizeAsync(user, Policy);
            isAuthorized = result.Succeeded;
        }

        if (!isAuthorized && !string.IsNullOrWhiteSpace(AnyPolicy))
        {
            var policies = AnyPolicy
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var policy in policies)
            {
                var result = await _authorizationService.AuthorizeAsync(user, policy);
                if (result.Succeeded)
                {
                    isAuthorized = true;
                    break;
                }
            }
        }

        if (!isAuthorized && !string.IsNullOrWhiteSpace(Permission))
        {
            isAuthorized = user.HasCharityPermission(Permission);
        }

        if (!isAuthorized)
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = null;
    }
}
