# تعديلات `Views/Shared/_Layout.cshtml`

## 1) قبل `@Html.Partial("~/Views/Shared/_vendor_scripts.cshtml")`
أضف:

```cshtml
@Html.Partial("~/Views/Shared/_RealtimeBootstrap.cshtml")
```

## 2) بعد `@Html.Partial("~/Views/Shared/_vendor_scripts.cshtml")`
أضف:

```cshtml
@Html.Partial("~/Views/Shared/_notification_scripts.cshtml")
```

فيكون الجزء السفلي من الـ layout هكذا:

```cshtml
@Html.Partial("~/Views/Shared/_right_sidebar.cshtml")
@Html.Partial("~/Views/Shared/_RealtimeBootstrap.cshtml")
@Html.Partial("~/Views/Shared/_vendor_scripts.cshtml")
@Html.Partial("~/Views/Shared/_notification_scripts.cshtml")
@RenderSection("scripts", required: false)
```
