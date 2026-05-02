
(function () {
    "use strict";
    if (window.SkoteNotifications) {
        return;
    }

    // == حالة وريفيرانس الواجهة ==
    const state = {
        conn: null,
        count: 0,
        inbox: [],             // {deliveryId, title, message, level, url, createdAtUtc, isRead}
        pageSize: 20
    };
    const ui = {
        badge: document.getElementById("notif-count"),
        list: document.getElementById("notif-list")
    };

    // == Helpers ==
    const esc = s => String(s ?? "").replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
    const mapLevel = lvl => {
        const l = (lvl || "info").toLowerCase();
        if (l === "error") return "danger";
        if (l === "success") return "success";
        if (l === "warning") return "warning";
        return "secondary";
    };
    function setCount(c) {
        state.count = c;
        if (!ui.badge) return;
        ui.badge.textContent = String(c);
        ui.badge.style.display = c > 0 ? "" : "none";
    }
    function fmt(dt) {
        try { return new Date(dt).toLocaleString(); } catch { return dt || ""; }
    }

    // نحدد الAPI Base من غير ما نغيّر أي متغيرات موجودة عندك
    function apiBase() {
        // لو عندك دالة global بنفس الاسم هنستخدمها كما هي
        if (typeof window.apiBase === "function") return window.apiBase();

        // أو من __APP__.realtimeBase إن موجودة
        if (window.__APP__?.realtimeBase) return String(window.__APP__.realtimeBase).replace(/\/+$/, "");

        // أو نستنتج الأصل (origin) من URL الhub الحالي
        const notifyUrl = window.__SIGNALR_HUBS__?.notify;
        if (notifyUrl) {
            try {
                const u = new URL(notifyUrl, window.__APP__);
                return u.origin; // مثال: https://localhost:6001
            } catch { }
        }
        return ""; // fallback (هيشتغل ريل تايم فقط لو ما قدرناش نحدد REST base)
    }

    // == عرض عنصر واحد ==
    function renderItem(x) {
        const el = document.createElement("a");
        el.href = x.url || "javascript:void(0);";
        el.className = "text-reset notification-item d-block " + (x.isRead ? "" : "unread");
        el.innerHTML = `
      <div class="d-flex">
        <div class="avatar-xs me-3">
          <span class="avatar-title bg-${mapLevel(x.level)} rounded-circle font-size-16">
            <i class="bx bx-bell"></i>
          </span>
        </div>
        <div class="flex-grow-1">
          <h6 class="mb-1">${esc(x.title || "Notification")}</h6>
          <div class="font-size-12 text-muted">
            <p class="mb-1">${esc(x.message || "")}</p>
            <p class="mb-0"><i class="mdi mdi-clock-outline"></i> <span class="time" data-iso="${x.createdAtUtc}">${relativeTime(x.createdAtUtc)}</span></p>
          </div>
        </div>
      </div>`;
        el.addEventListener("click", (ev) => {
            if (!x.url) ev.preventDefault();
            if (!x.isRead && x.deliveryId) {
                markRead(x.deliveryId);
                x.isRead = true;
                el.classList.remove("unread");
                // عدّل العدّاد فورًا
                setCount(Math.max(0, state.count - 1));
            }
        });
        return el;
    }

    // == إعادة رسم القائمة كاملة ==
    function renderList() {
        if (!ui.list) return;
        //ui.list.innerHTML = "";
        state.inbox.forEach(x => ui.list.appendChild(renderItem(x)));
        const unread = state.inbox.filter(i => !i.isRead).length;
        setCount(unread);
    }
    function showBootstrapToast({ title = "Notification", message = "", level = "info" } = {}) {
        // جهّز الحاوية مرة واحدة
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            document.body.appendChild(container);
        }

        // خريطة ألوان Bootstrap
        const map = { error: 'danger', success: 'success', warning: 'warning', info: 'secondary' };
        const cls = map[(level || 'info').toLowerCase()] || 'secondary';

        // عنصر التوست
        const wrap = document.createElement('div');
        wrap.className = `toast app-toast text-bg-${cls} border-0`;
        wrap.setAttribute('role', 'alert');
        wrap.setAttribute('aria-live', 'assertive');
        wrap.setAttribute('aria-atomic', 'true');
        wrap.innerHTML = `
    <div class="toast-header border-0">
      <strong class="me-auto">${title}</strong>
      <small class="text-muted">now</small>
      <button type="button" class="btn-close ms-2" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body">${message}</div>
  `;

        container.appendChild(wrap);

        // فعّل Toast (Bootstrap 5)
        try {
            const toast = new bootstrap.Toast(wrap, { delay: 4000, autohide: true });
            toast.show();
            // إزالة من الـ DOM بعد الاختفاء
            wrap.addEventListener('hidden.bs.toast', () => wrap.remove());
        } catch {
            // في حال bootstrap.Toast مش متاح لأي سبب
            wrap.style.display = 'block';
            setTimeout(() => wrap.remove(), 4000);
        }
    }
    // == إضافة إشعار واحد (ريال تايم) ==
    function showInviteToast(title, message, eventId) {
        let container = document.getElementById('toast-container');
        if (!container) { container = document.createElement('div'); container.id = 'toast-container'; document.body.appendChild(container); }

        const wrap = document.createElement('div');
        wrap.className = 'toast app-toast text-bg-secondary border-0';
        wrap.setAttribute('role', 'alert'); wrap.setAttribute('aria-live', 'assertive'); wrap.setAttribute('aria-atomic', 'true');
        wrap.innerHTML = `
    <div class="toast-header border-0">
      <strong class="me-auto">${title}</strong>
      <button type="button" class="btn-close ms-2" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body">
      <div class="mb-2">${message}</div>
      <div class="d-flex gap-2">
        <button class="btn btn-sm btn-success" data-act="accept">Accept</button>
        <button class="btn btn-sm btn-outline-danger" data-act="decline">Decline</button>
        <button class="btn btn-sm btn-outline-secondary" data-act="tentative">Tentative</button>
      </div>
    </div>
  `;
        container.appendChild(wrap);

        wrap.querySelector('[data-act="accept"]').addEventListener('click', () => respondToInvite(eventId, "accepted"));
        wrap.querySelector('[data-act="decline"]').addEventListener('click', () => respondToInvite(eventId, "declined"));
        wrap.querySelector('[data-act="tentative"]').addEventListener('click', () => respondToInvite(eventId, "tentative"));

        try {
            const toast = new bootstrap.Toast(wrap, { delay: 60000, autohide: false });
            toast.show();
            wrap.addEventListener('hidden.bs.toast', () => wrap.remove());
        } catch {
            wrap.style.display = 'block';
            setTimeout(() => wrap.remove(), 60000);
        }
    }
    async function markAllReadBulk() {
        const base = apiBase();
        const token = await TokenProvider.getToken();
        await fetch(`${base}/notify/read/all`, { method: 'POST', headers: { Authorization: 'Bearer ' + token } });
        SkoteNotifications.state.inbox.forEach(i => i.isRead = true);
        SkoteNotifications.refresh();
    }

    function addRealtimeItem(p) {
        // payload ممكن ما يكونش فيه deliveryId — نولده مؤقتًا
        const item = {
            deliveryId: p.deliveryId || (crypto.randomUUID?.() || String(Date.now())),
            title: p.title || "Notification",
            message: p.message || "",
            level: p.level || "info",
            url: p.url || null,
            createdAtUtc: new Date().toISOString(),
            isRead: false
        };
        state.inbox.unshift(item);
        if (state.inbox.length > state.pageSize) state.inbox.length = state.pageSize;
        renderList();

        // Toast (لو موجود)
        if (p && (p.kind === "calendar-invite" || p.meta?.eventId)) {
            const evId = p.meta?.eventId || p.eventId; // دعم مرن
            showInviteToast(p.title || "Calendar Invite", p.message || "", evId);
        } else {
            showBootstrapToast({ title: p.title || "Notification", message: p.message || "", level: p.level || "info" });
        }
    }
    
    // == API Calls ==
    async function respondToInvite(eventId, status) {
        try {
            const base = apiBase();
            const token = await TokenProvider.getToken();
            const res = await fetch(`${base}/calendar/respond`, {
                method: "POST",
                headers: { Authorization: "Bearer " + token, "Content-Type": "application/json" },
                body: JSON.stringify({ eventId, status })
            });
            if (!res.ok) throw new Error("respond failed");
            showBootstrapToast({ title: "Calendar", message: `تم إرسال ردّك (${status})`, level: "success" });
        } catch {
            showBootstrapToast({ title: "Calendar", message: "تعذر إرسال الرد", level: "error" });
        }
    }
    async function fetchInbox({ unreadOnly = false, take = state.pageSize } = {}) {
        const base = apiBase();
        if (!base) { // لو مش قادرين نحدد REST base، نكتفي بالريال تايم
            renderList();
            return;
        }
        try {
            const token = await TokenProvider.getToken();
            const url = `${base}/notify/my?unreadOnly=${unreadOnly}&take=${take}`;
            const res = await fetch(url, { headers: { Authorization: "Bearer " + token } });
            const data = await res.json();
            state.inbox = (data || []).map(x => ({
                deliveryId: x.deliveryId || x.DeliveryId,
                title: x.title || x.Title || "Notification",
                message: x.message || x.Message || "",
                level: x.level || x.Level || "info",
                url: x.url || x.Url || null,
                createdAtUtc: x.createdAtUtc || x.CreatedAtUtc,
                isRead: !!(x.isRead ?? x.IsRead)
            }));
            renderList();
        } catch (e) {
            console.warn("[notify] fetchInbox failed:", e);
        }
    }
    async function markAllRead() {
        // علّم محليًا
        const unread = state.inbox.filter(i => !i.isRead);
        unread.forEach(i => i.isRead = true);
        renderList();
        // نداءات خلفية (واحد واحد – أو اعمل Endpoint مخصص لو حبيت)
        for (const it of unread) {
            try { await markRead(it.deliveryId); } catch { }
        }
    }

    async function loadMore() {
        state.pageSize += 20; // زوّد الصفحة
        await fetchInbox({ unreadOnly: false, take: state.pageSize });
    }

    async function markRead(deliveryId) {
        const base = apiBase();
        if (!base) return;
        try {
            const token = await TokenProvider.getToken();
            await fetch(`${base}/notify/read`, {
                method: "POST",
                headers: { Authorization: "Bearer " + token, "Content-Type": "application/json" },
                body: JSON.stringify(deliveryId)
            });
        } catch (e) {
            console.warn("[notify] markRead failed:", e);
        }
    }

    // == بدء الاتصال والربط ==
    async function init() {
        // 1) تحميل البريد الحالي وتحديث الـ badge
        await fetchInbox({ unreadOnly: false, take: state.pageSize });

        // 2) اتصال هَب (بدون تغيير طريقة تحديد URL عندك)
        const notify = await SignalR.get('notify', {
            tokenFactory: () => TokenProvider.getToken()
            // يعتمد على window.__SIGNALR_HUBS__.notify كما هو في مشروعك
        });
        state.conn = notify;

        // انضمام للغرفة الشخصية
        try { await notify.invoke("JoinPersonal"); } catch { }
        const btnMarkAll = document.getElementById('notif-markall');
        const btnLoadMore = document.getElementById('notif-loadmore');
        btnMarkAll?.addEventListener('click', (e) => { e.preventDefault(); markAllRead(); });
        btnLoadMore?.addEventListener('click', async (e) => { e.preventDefault(); await loadMore(); });
        // Backward-compat: لو السيرفر بيبعت ReceiveNotification (قديم)
        notify.on("ReceiveNotification", payload => {
            console.log("[notify] ReceiveNotification", payload);
            // أضف عنصر مبسّط (زي سكربتك القديم)
            addRealtimeItem({
                title: payload?.title,
                message: payload?.message,
                level: payload?.level,
                url: payload?.url
            });
        });

        // الحدث الجديد القياسي: "Notify"
        notify.on("Notify", (p) => {
            console.log("[notify] Notify", p);
            addRealtimeItem(p);
        });

        // مثال الانضمام لتوبيك (لو حبيت)
        // await notify.invoke("JoinTopic", "it");
    }
   
    // وقت نسبي (سريع وخفيف)
    function relativeTime(iso) {
        try {
            const d = new Date(iso), diff = (Date.now() - d.getTime()) / 1000;
            if (diff < 60) return "just now";
            if (diff < 3600) return Math.floor(diff / 60) + " min ago";
            if (diff < 86400) return Math.floor(diff / 3600) + " h ago";
            return d.toLocaleString();
        } catch { return iso || ""; }
    }
    setInterval(() => {
        document.querySelectorAll('#notif-list .time[data-iso]').forEach(el => {
            const iso = el.getAttribute('data-iso');
            el.textContent = relativeTime(iso);
        });
    }, 60000);
    init().catch(err => console.error("[notify] init failed:", err));
    Object.defineProperty(window, 'SkoteNotifications', {
        value: {
            init,
            refresh: fetchInbox,
            markRead,
            markAllRead,     // جديد
            loadMore,        // جديد
            get state() { return state; }
        },
        writable: false
    });
})();
//    (function(){
//        "use strict";
//    const state = {conn: null, count: 0 };
//    const ui = {
//        badge: document.getElementById("notif-count"), // <span id="notif-count" class="badge bg-danger" style="display:none">0</span>
//    list:  document.getElementById("notif-list")   // <div id="notif-list" class="dropdown-menu dropdown-menu-end"></div>
//  };
//  const esc = s => String(s ?? "").replace(/[&<>"']/g, m => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;', "'":'&#39;'}[m]));
//        function setCount(c){state.count = c; if(ui.badge){ui.badge.textContent = String(c); ui.badge.style.display=c>0?"":"none"; } }
//        function addItem(p){
//    const el = document.createElement("div");
//        el.className = "dropdown-item";
//        el.innerHTML = `<div class="d-flex">
//            <div class="me-2"><i class="mdi mdi-bell-outline"></i></div>
//            <div><div class="fw-semibold">${esc(p?.title ?? "Notification")}</div>
//                <small class="text-muted">${esc(p?.message ?? "")}</small></div></div>`;
//        ui.list?.prepend(el); setCount(state.count+1);
//  }
//        async function init(){
//            const notify = await SignalR.get('notify', {
//                tokenFactory: () => TokenProvider.getToken()
//            });// يعتمد على window.__SIGNALR_HUBS__.notify
//       state.conn = notify;
//    notify.on("ReceiveNotification", payload => {console.log("[notify]", payload); addItem(payload||{ }); });
//            await notify.invoke("JoinPersonal");
//    notify.on("Notify", (p) => {
//          console.log("Notify:", p);
//          // اربطها مع UI Skote (toastr أو أي Toast موجود)
//          if (window.toastr) {
//            const level = (p.level || "info").toLowerCase();
//            const fn = level === "error" ? "error"
//                    : level === "success" ? "success"
//                    : level === "warning" ? "warning" : "info";
//            toastr[fn](p.message || "", p.title || "Notification");
//          }
//        });
//    // مثال: الانضمام لتوبيك
//    // await notify.invoke("JoinTopic", "it");
//  }
//  init().catch(err => console.error("[notify] init failed:", err));
//})();
    