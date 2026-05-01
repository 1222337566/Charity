(function () {
    "use strict";

    const listEl = document.getElementById("notification-center-list");
    if (!listEl) return;

    const emptyEl = document.getElementById("notification-center-empty");
    const totalEl = document.getElementById("notification-center-total");
    const unreadEl = document.getElementById("notification-center-unread");
    const unreadBadgeEl = document.getElementById("notification-center-unread-badge");
    const lastSyncEl = document.getElementById("notification-center-last-sync");
    const refreshBtn = document.getElementById("notification-center-refresh");
    const markAllBtn = document.getElementById("notification-center-mark-all");
    const filterAllBtn = document.getElementById("notification-center-filter-all");
    const filterUnreadBtn = document.getElementById("notification-center-filter-unread");

    let currentUnreadOnly = false;

    const esc = s => String(s ?? "").replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
    const mapLevel = lvl => {
        const l = String(lvl || "info").toLowerCase();
        if (l === "error" || l === "danger") return "danger";
        if (l === "success") return "success";
        if (l === "warning") return "warning";
        return "secondary";
    };

    function relativeTime(iso) {
        try {
            const d = new Date(iso);
            const diff = Math.floor((Date.now() - d.getTime()) / 1000);
            if (diff < 60) return "الآن";
            if (diff < 3600) return `${Math.floor(diff / 60)} دقيقة`;
            if (diff < 86400) return `${Math.floor(diff / 3600)} ساعة`;
            return d.toLocaleString();
        } catch {
            return iso || "";
        }
    }

    function render(items) {
        listEl.innerHTML = "";

        if (!items || items.length === 0) {
            emptyEl.classList.remove("d-none");
            totalEl.textContent = "0";
            unreadEl.textContent = "0";
            unreadBadgeEl.textContent = "0 غير مقروء";
            return;
        }

        emptyEl.classList.add("d-none");
        const unreadCount = items.filter(x => !x.isRead).length;
        totalEl.textContent = String(items.length);
        unreadEl.textContent = String(unreadCount);
        unreadBadgeEl.textContent = `${unreadCount} غير مقروء`;

        for (const item of items) {
            const wrapper = document.createElement("div");
            wrapper.className = `border rounded p-3 ${item.isRead ? 'bg-light' : 'border-primary-subtle'}`;
            wrapper.innerHTML = `
                <div class="d-flex justify-content-between align-items-start gap-3">
                    <div class="flex-grow-1">
                        <div class="d-flex align-items-center gap-2 mb-1">
                            <span class="badge bg-${mapLevel(item.level)}-subtle text-${mapLevel(item.level)}">${esc(item.level || 'info')}</span>
                            ${item.isRead ? '<span class="badge bg-light text-muted">مقروء</span>' : '<span class="badge bg-primary-subtle text-primary">جديد</span>'}
                        </div>
                        <h6 class="mb-1">${esc(item.title || 'Notification')}</h6>
                        <div class="text-muted mb-2">${esc(item.message || '')}</div>
                        <div class="small text-muted">${relativeTime(item.createdAtUtc)}</div>
                    </div>
                    <div class="d-flex flex-column gap-2">
                        ${item.url ? `<a class="btn btn-sm btn-outline-primary" href="${item.url}">فتح</a>` : ''}
                        ${!item.isRead && item.deliveryId ? `<button type="button" class="btn btn-sm btn-light" data-delivery-id="${item.deliveryId}">تعليم كمقروء</button>` : ''}
                    </div>
                </div>`;
            listEl.appendChild(wrapper);
        }

        listEl.querySelectorAll("button[data-delivery-id]").forEach(btn => {
            btn.addEventListener("click", async () => {
                const deliveryId = btn.getAttribute("data-delivery-id");
                if (!deliveryId || !window.SkoteNotifications?.markRead) return;
                await window.SkoteNotifications.markRead(deliveryId);
                await refresh();
            });
        });
    }

    async function refresh() {
        if (!window.SkoteNotifications?.refresh) return;
        await window.SkoteNotifications.refresh({ unreadOnly: currentUnreadOnly, take: 100 });
        const items = (window.SkoteNotifications.state?.inbox || []).filter(x => currentUnreadOnly ? !x.isRead : true);
        render(items);
        lastSyncEl.textContent = new Date().toLocaleTimeString();
    }

    refreshBtn?.addEventListener("click", refresh);
    markAllBtn?.addEventListener("click", async () => {
        if (!window.SkoteNotifications?.markAllRead) return;
        await window.SkoteNotifications.markAllRead();
        await refresh();
    });
    filterAllBtn?.addEventListener("click", async () => {
        currentUnreadOnly = false;
        filterAllBtn.classList.add("active");
        filterUnreadBtn.classList.remove("active");
        await refresh();
    });
    filterUnreadBtn?.addEventListener("click", async () => {
        currentUnreadOnly = true;
        filterUnreadBtn.classList.add("active");
        filterAllBtn.classList.remove("active");
        await refresh();
    });

    window.addEventListener("focus", refresh);
    document.addEventListener("DOMContentLoaded", refresh);
})();
