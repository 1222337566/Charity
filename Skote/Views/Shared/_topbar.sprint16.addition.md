# تعديل بسيط داخل `Views/Shared/_topbar.cshtml`

استبدل Footer الخاص بدروب داون التنبيهات:

```html
<div class="p-2 border-top text-center bg-light">
    <a id="notif-loadmore" href="#" class="small text-muted">Load more</a>
</div>
```

بالتالي:

```html
<div class="p-2 border-top d-flex justify-content-between align-items-center bg-light">
    <a id="notif-loadmore" href="#" class="small text-muted">Load more</a>
    <a asp-controller="NotificationCenter" asp-action="Index" class="small text-primary">View all</a>
</div>
```

> لو كنت تريدها بالعربية، غيّر النص إلى: `عرض الكل`.
