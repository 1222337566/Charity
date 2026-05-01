using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Skote.Helpers;

public sealed class CharityControllerAuthorizationConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            ApplyController(controller);
        }
    }

    private static void ApplyController(ControllerModel controller)
    {
        var controllerName = controller.ControllerName;
        var entry = CharityControllerAccessMap.GetEntry(controllerName);
        if (entry is null)
        {
            return;
        }

        var controllerHasAllowAnonymous = controller.Attributes.OfType<AllowAnonymousAttribute>().Any();
        var controllerHasAuthorize = controller.Attributes.OfType<IAuthorizeData>().Any();

        if (!controllerHasAllowAnonymous && !controllerHasAuthorize && !string.IsNullOrWhiteSpace(entry.ViewPolicy))
        {
            controller.Filters.Add(new AuthorizeFilter(entry.ViewPolicy));
        }

        if (controllerHasAllowAnonymous || controllerHasAuthorize)
        {
            return;
        }

        foreach (var action in controller.Actions)
        {
            var actionHasAllowAnonymous = action.Attributes.OfType<AllowAnonymousAttribute>().Any();
            var actionHasAuthorize = action.Attributes.OfType<IAuthorizeData>().Any();
            if (actionHasAllowAnonymous || actionHasAuthorize)
            {
                continue;
            }

            var actionPolicy = CharityControllerAccessMap.GetPolicy(controllerName, action.ActionName);
            if (!string.IsNullOrWhiteSpace(actionPolicy) && !string.Equals(actionPolicy, entry.ViewPolicy, StringComparison.OrdinalIgnoreCase))
            {
                action.Filters.Add(new AuthorizeFilter(actionPolicy));
            }
        }
    }
}
