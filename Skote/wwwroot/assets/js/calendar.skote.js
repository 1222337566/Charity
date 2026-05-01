(function () {
    "use strict";

    // ===== Helpers =====
    const $ = sel => document.querySelector(sel);
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

    //const apiBase = () => {
    //    if (typeof window.apiBase === "function") return window.apiBase();
    //    if (window.__APP__?.realtimeBase) return String(window.__APP__.realtimeBase).replace(/\/+$/, "");
    //    const notifyUrl = window.__SIGNALR_HUBS__?.notify;
    //    if (notifyUrl) try { return new URL(notifyUrl, location.origin).origin; } catch { }
    //    return "";
    //};
    const toIso = (s) => s ? new Date(s).toISOString() : null;
    const pad = n => n < 10 ? "0" + n : "" + n;
    const toLocalInputValue = (iso) => {
        if (!iso) return "";
        const d = new Date(iso);
        const y = d.getFullYear(), m = pad(d.getMonth() + 1), da = pad(d.getDate());
        const h = pad(d.getHours()), mi = pad(d.getMinutes());
        return `${y}-${m}-${da}T${h}:${mi}`;
    };

    // ===== FullCalendar instance =====
    let calendar;
    let modal, modalApi;

    async function fetchEvents(info, success, failure) {
        try {
            const base = apiBase();
            const token = await TokenProvider.getToken();
            // نتوقع Endpoints:
            // GET /calendar/events?start=..&end=..
            const url = `${base}/calendar/events?start=${encodeURIComponent(info.startStr)}&end=${encodeURIComponent(info.endStr)}`;
            const res = await fetch(url, { headers: { Authorization: "Bearer " + token } });
            if (!res.ok) throw new Error(res.statusText);
            const data = await res.json();
            // نتوقع { id, title, startUtc, endUtc, allDay?, color? ... }
            const events = (data || []).map(ev => ({
                id: ev.id || ev.Id,
                title: ev.title || ev.Title,
                start: ev.startUtc || ev.StartUtc,
                end: ev.endUtc || ev.EndUtc,
                allDay: !!(ev.allDay ?? ev.AllDay),
                color: ev.color || ev.Color || undefined
            }));
            success(events);
        } catch (e) {
            console.warn("[calendar] fetch failed:", e);
            failure(e);
        }
    }

    async function createEvent(payload) {
        const base = apiBase();
        const token = await TokenProvider.getToken();
        // POST /calendar/create
        const res = await fetch(`${base}/calendar/create`, {
            method: "POST",
            headers: { Authorization: "Bearer " + token, "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        if (!res.ok) throw new Error("create failed");
        return res.json(); // { ok:true, eventId:"..." }
    }

    async function updateEvent(payload) {
        const base = apiBase();
        const token = await TokenProvider.getToken();
        // PUT /calendar/update
        const res = await fetch(`${base}/calendar/update`, {
            method: "PUT",
            headers: { Authorization: "Bearer " + token, "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        if (!res.ok) throw new Error("update failed");
        return res.json();
    }

    async function deleteEvent(eventId) {
        const base = apiBase();
        const token = await TokenProvider.getToken();
        // DELETE /calendar/{id}
        const res = await fetch(`${base}/calendar/${eventId}`, {
            method: "DELETE",
            headers: { Authorization: "Bearer " + token }
        });
        if (!res.ok) throw new Error("delete failed");
        return res.json();
    }

    function openModal({ id = null, title = "", start = "", end = "", allDay = false }) {
        $("#evt-id").value = id || "";
        $("#evt-title").value = title || "";
        $("#evt-start").value = toLocalInputValue(start);
        $("#evt-end").value = toLocalInputValue(end);
        $("#evt-delete").style.display = id ? "" : "none";
        modalApi.show();
    }

    function closeModal() { modalApi.hide(); }

    function bindModal() {
        modal = $("#eventModal");
        modalApi = new bootstrap.Modal(modal, { backdrop: "static" });

        // حفظ
        $("#eventForm").addEventListener("submit", async (e) => {
            e.preventDefault();
            const id = $("#evt-id").value || null;
            const title = $("#evt-title").value?.trim();
            const start = toIso($("#evt-start").value);
            const end = toIso($("#evt-end").value);

            if (!title || !start) { showBootstrapToast({ title: "Calendar", message: "العنوان والبداية مطلوبان", level: "error" }); return; }

            try {
                if (!id) {
                    // إنشاء
                    const dto = { title, startUtc: start, endUtc: end, attendeesUserIds: [] }; // لو حابب: attendeesUserIds
                    await createEvent(dto);
                } else {
                    // تعديل
                    const dto = { id, title, startUtc: start, endUtc: end };
                    await updateEvent(dto);
                }
                closeModal();
                calendar.refetchEvents();
                showBootstrapToast({ title: "Calendar", message: "تم الحفظ", level: "success" });
            } catch (err) {
                console.error(err);
                showBootstrapToast({ title: "Calendar", message: "فشل الحفظ", level: "error" });
            }
        });

        // حذف
        $("#evt-delete").addEventListener("click", async () => {
            const id = $("#evt-id").value;
            if (!id) return;
            try {
                await deleteEvent(id);
                closeModal();
                calendar.refetchEvents();
                showBootstrapToast({ title: "Calendar", message: "تم الحذف", level: "success" });
            } catch (err) {
                console.error(err);
                showBootstrapToast({ title: "Calendar", message: "فشل الحذف", level: "error" });
            }
        });
    }

    // Bootstrap Toast fallback (لو مفيش Toastr)
    function showBootstrapToast({ title = "Notification", message = "", level = "info" } = {}) {
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div'); container.id = 'toast-container';
            container.style.position = 'fixed'; container.style.top = '1rem'; container.style.right = '1rem'; container.style.zIndex = '1080'; document.body.appendChild(container);
        }
        const map = { error: 'danger', success: 'success', warning: 'warning', info: 'secondary' };
        const cls = map[(level || 'info').toLowerCase()] || 'secondary';
        const wrap = document.createElement('div');
        wrap.className = `toast app-toast text-bg-${cls} border-0`;
        wrap.setAttribute('role', 'alert'); wrap.setAttribute('aria-live', 'assertive'); wrap.setAttribute('aria-atomic', 'true');
        wrap.innerHTML = `
      <div class="toast-header border-0">
        <strong class="me-auto">${title}</strong>
        <button type="button" class="btn-close ms-2" data-bs-dismiss="toast" aria-label="Close"></button>
      </div>
      <div class="toast-body">${message}</div>`;
        container.appendChild(wrap);
        try { const t = new bootstrap.Toast(wrap, { delay: 3500, autohide: true }); t.show(); wrap.addEventListener('hidden.bs.toast', () => wrap.remove()); }
        catch { wrap.style.display = 'block'; setTimeout(() => wrap.remove(), 3500); }
    }

    async function initCalendar() {
        bindModal();

        const el = document.getElementById("fc-calendar");
        if (!el) { console.warn("[calendar] #fc-calendar not found"); return; }
        calendar = new FullCalendar.Calendar(el, {
            // ===== مهم: ثيم Bootstrap 5 =====
            themeSystem: 'bootstrap5',

            timeZone: 'local',
            initialView: 'dayGridMonth',
            locale: (document.documentElement.lang?.startsWith('ar') ? 'ar' : 'en'),
            direction: (document.dir || (document.documentElement.lang?.startsWith('ar') ? 'rtl' : 'ltr')),
            height: 'auto',

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
            },
            buttonText: { today: 'اليوم', month: 'شهر', week: 'أسبوع', day: 'يوم', list: 'قائمة' },

            selectable: true,
            selectMirror: true,
            editable: true,
            dayMaxEventRows: true,

            // نفس دالة fetchEvents اللي عندك
            events: fetchEvents,

            // نضيف “أسماء كلاس” حسب level/kind جايين من السيرفر
            eventClassNames: (arg) => {
                const cls = [];
                const ex = arg.event.extendedProps || {};
                const level = (ex.level || '').toLowerCase();
                if (level === 'success') cls.push('fc-event-level-success');
                else if (level === 'warning') cls.push('fc-event-level-warning');
                else if (level === 'error' || level === 'danger') cls.push('fc-event-level-danger');
                else cls.push('fc-event-level-info');

                const kind = (ex.kind || '').toLowerCase();
                if (kind) {
                    if (kind.includes('invite')) cls.push('fc-event-kind-invite');
                    else if (kind.includes('remind')) cls.push('fc-event-kind-reminder');
                    else if (kind.includes('update')) cls.push('fc-event-kind-update');
                }
                return cls;
            },

            // لو عايز أيقونة صغيرة قبل العنوان حسب النوع
            eventContent: (arg) => {
                const ex = arg.event.extendedProps || {};
                const kind = (ex.kind || '').toLowerCase();
                let icon = null;
                if (kind.includes('invite')) icon = '<i class="bx bx-envelope-open me-1"></i>';
                else if (kind.includes('remind')) icon = '<i class="bx bx-time-five me-1"></i>';
                else if (kind.includes('update')) icon = '<i class="bx bx-refresh me-1"></i>';

                const esc = (s) => String(s ?? "")
                    .replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));

                const titleHtml = (icon ? icon : '') + esc(arg.event.title || '');
                return { html: `<span class="fw-600">${titleHtml}</span>` };
            },

            select: (arg) => {
                openModal({ title: '', start: arg.startStr, end: arg.endStr, allDay: arg.allDay });
            },

            eventClick: (info) => {
                const e = info.event;
                openModal({
                    id: e.id,
                    title: e.title,
                    start: e.start?.toISOString(),
                    end: e.end?.toISOString(),
                    allDay: e.allDay
                });
            },

            eventDrop: async (info) => {
                try {
                    await updateEvent({
                        id: info.event.id,
                        title: info.event.title,
                        startUtc: info.event.start?.toISOString(),
                        endUtc: info.event.end?.toISOString()
                    });
                    showBootstrapToast({ title: "Calendar", message: "تم التحديث", level: "success" });
                } catch (e) {
                    info.revert();
                    showBootstrapToast({ title: "Calendar", message: "فشل التحديث", level: "error" });
                }
            },

            eventResize: async (info) => {
                try {
                    await updateEvent({
                        id: info.event.id,
                        title: info.event.title,
                        startUtc: info.event.start?.toISOString(),
                        endUtc: info.event.end?.toISOString()
                    });
                    showBootstrapToast({ title: "Calendar", message: "تم التحديث", level: "success" });
                } catch (e) {
                    info.revert();
                    showBootstrapToast({ title: "Calendar", message: "فشل التحديث", level: "error" });
                }
            }
        });
        //calendar = new FullCalendar.Calendar(el, {
        //    timeZone: 'local',
        //    initialView: 'dayGridMonth',
        //    locale: document.documentElement.lang?.startsWith('ar') ? 'ar' : 'en',
        //    direction: document.dir || (document.documentElement.lang?.startsWith('ar') ? 'rtl' : 'ltr'),
        //    height: 'auto',
        //    headerToolbar: {
        //        left: 'prev,next today',
        //        center: 'title',
        //        right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        //    },
        //    buttonText: { today: 'اليوم', month: 'شهر', week: 'أسبوع', day: 'يوم', list: 'قائمة' },

        //    selectable: true,
        //    selectMirror: true,
        //    editable: true,        // drag & drop + resize
        //    dayMaxEventRows: true,

        //    // تحميل الأحداث
        //    events: fetchEvents,

        //    // إنشاء سريع بالاختيار
        //    select: (arg) => {
        //        openModal({
        //            title: '',
        //            start: arg.startStr,
        //            end: arg.endStr,
        //            allDay: arg.allDay
        //        });
        //    },

        //    // عند الضغط على حدث → فتح تعديل
        //    eventClick: (info) => {
        //        const e = info.event;
        //        openModal({
        //            id: e.id,
        //            title: e.title,
        //            start: e.start?.toISOString(),
        //            end: e.end?.toISOString(),
        //            allDay: e.allDay
        //        });
        //    },

        //    // سحب/إفلات → تحديث
        //    eventDrop: async (info) => {
        //        try {
        //            await updateEvent({
        //                id: info.event.id,
        //                title: info.event.title,
        //                startUtc: info.event.start?.toISOString(),
        //                endUtc: info.event.end?.toISOString()
        //            });
        //            showBootstrapToast({ title: "Calendar", message: "تم التحديث", level: "success" });
        //        } catch (e) {
        //            info.revert();
        //            showBootstrapToast({ title: "Calendar", message: "فشل التحديث", level: "error" });
        //        }
        //    },

        //    eventResize: async (info) => {
        //        try {
        //            await updateEvent({
        //                id: info.event.id,
        //                title: info.event.title,
        //                startUtc: info.event.start?.toISOString(),
        //                endUtc: info.event.end?.toISOString()
        //            });
        //            showBootstrapToast({ title: "Calendar", message: "تم التحديث", level: "success" });
        //        } catch (e) {
        //            info.revert();
        //            showBootstrapToast({ title: "Calendar", message: "فشل التحديث", level: "error" });
        //        }
        //    }
        //});

        calendar.render();

        // أزرار أعلى الكارد
        document.getElementById("btn-today")?.addEventListener("click", () => calendar.today());
        document.getElementById("btn-new")?.addEventListener("click", () => openModal({}));

        // تحديث لحظي عند وصول إشعارات التقويم
        try {
            const notify = await SignalR.get('notify', { tokenFactory: () => TokenProvider.getToken() });
            notify.on("Notify", (p) => {
                if (p?.kind?.startsWith?.("calendar-") || p?.meta?.eventId) {
                    calendar.refetchEvents();
                }
            });
            // دعماً للحدث القديم:
            notify.on("ReceiveNotification", (p) => {
                if (p?.kind?.startsWith?.("calendar-") || p?.meta?.eventId) {
                    calendar.refetchEvents();
                }
            });
            await notify.invoke("JoinPersonal").catch(() => { });
        } catch (e) {
            console.warn("[calendar] realtime not attached:", e);
        }
    }

    // ابدأ بعد جاهزية الـ DOM
    if (document.readyState === "loading")
        document.addEventListener("DOMContentLoaded", initCalendar);
    else
        initCalendar();

})();
