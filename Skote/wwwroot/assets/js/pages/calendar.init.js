/*
Template Name: Skote - Admin & Dashboard Template
Author: Themesbrand
File: Calendar init js (API-powered)
*/

'use strict';
// ==== Modal/Date helpers ====

/* eslint-disable require-jsdoc */
/* eslint-env jquery */
/* global moment, tui, chance */
/* global findCalendar, CalendarList */

(function (window, Calendar) {
    var cal, resizeThrottled;
    var useCreationPopup = true;
    var useDetailPopup = true;
    var datePicker, selectedCalendar;

    // ===== Helpers =====
    const esc = s => String(s ?? '').replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
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
    async function getTokenMaybe() {
        try { return await (window.TokenProvider?.getToken?.() ?? ''); } catch { return ''; }
    }
    function mapEventToSchedule(e) {
        const id = e.id || e.Id;
        const title = e.title || e.Title || 'Untitled';
        const startUtc = e.startUtc || e.StartUtc;
        const endUtc = e.endUtc || e.EndUtc;
        const isAllDay = !!(e.isAllDay ?? e.IsAllDay);
        const calendarId = (e.calendarId || e.CalendarId || CalendarList?.[0]?.id || '1') + '';
        const location = e.location || e.Location || '';
        const attendees = e.attendees || e.Attendees || [];
        const isBusy = !!(e.isBusy ?? e.IsBusy);

        const calInfo = findCalendar(calendarId) || CalendarList[0];
        return {
            id: String(id),
            calendarId: calendarId,
            title: title,
            category: isAllDay ? 'allday' : 'time',
            isAllDay: isAllDay,
            start: startUtc,
            end: endUtc,
            location: location,
            attendees: attendees,                // TUI يقرأها ويعرضها في الـpopup
            state: isBusy ? 'Busy' : 'Free',
            color: calInfo?.color,
            bgColor: calInfo?.bgColor,
            dragBgColor: calInfo?.dragBgColor,
            borderColor: calInfo?.borderColor
        };
    }

    async function fetchSchedules(rangeStart, rangeEnd) {
        const base = apiBase();
        const start = rangeStart?.toDate ? rangeStart.toDate() : new Date(rangeStart);
        const end = rangeEnd?.toDate ? rangeEnd.toDate() : new Date(rangeEnd);

        const url = `${base}/calendar/events?start=${encodeURIComponent(start.toISOString())}&end=${encodeURIComponent(end.toISOString())}`;
        const headers = {};
        const t = await getTokenMaybe();
        if (t) headers['Authorization'] = 'Bearer ' + t;

        const res = await fetch(url, { headers, credentials: 'include' });
        if (!res.ok) throw new Error('Load events failed: ' + res.status);
        const data = await res.json();
        return (data || []).map(mapEventToSchedule);
    }
    async function respondEventOnApi(eventId, status) {
        const base = (typeof window.apiBase === 'function' ? window.apiBase() : (window.__APP__?.realtimeBase || '')).replace(/\/+$/, '');
        const headers = { 'Content-Type': 'application/json' };
        const t = await (window.TokenProvider?.getToken?.() ?? '');
        if (t) headers.Authorization = 'Bearer ' + t;

        const res = await fetch(`${base}/calendar/respond`, {
            method: 'POST',
            headers, credentials: 'include',
            body: JSON.stringify({ eventId, status })
        });
        if (!res.ok) throw new Error('respond failed');
        return res.json();
    }
    async function createEventOnApi(dto) {
        const base = apiBase();
        const headers = { 'Content-Type': 'application/json' };
        const t = await getTokenMaybe();
        if (t) headers['Authorization'] = 'Bearer ' + t;

        const res = await fetch(`${base}/calendar/create`, {
            method: 'POST',
            headers, credentials: 'include',
            body: JSON.stringify(dto)
        });
        if (!res.ok) throw new Error('create failed');
        return res.json(); // { ok:true, eventId:"..." }
    }

    async function updateEventOnApi(dto) {
        const base = apiBase();
        const headers = { 'Content-Type': 'application/json' };
        const t = await getTokenMaybe();
        if (t) headers['Authorization'] = 'Bearer ' + t;

        const res = await fetch(`${base}/calendar/update`, {
            method: 'PUT',
            headers, credentials: 'include',
            body: JSON.stringify(dto)
        });
        if (!res.ok) throw new Error('update failed');
        return res.json();
    }

    async function deleteEventOnApi(id) {
        const base = apiBase();
        const headers = {};
        const t = await getTokenMaybe();
        if (t) headers['Authorization'] = 'Bearer ' + t;

        const res = await fetch(`${base}/calendar/${encodeURIComponent(id)}`, {
            method: 'DELETE', headers, credentials: 'include'
        });
        if (!res.ok) throw new Error('delete failed');
        return res.json();
    }

    // ===== Calendar init =====
    cal = new Calendar('#calendar', {
        defaultView: 'month',
        useCreationPopup: useCreationPopup,
        useDetailPopup: useDetailPopup, // هنستفيد من الـpopup الافتراضي (بيعرض location/attendees)
        calendars: CalendarList,
        template: {
            milestone: function (model) {
                return '<span class="calendar-font-icon ic-milestone-b"></span> <span style="background-color: ' + model.bgColor + '">' + model.title + '</span>';
            },
            allday: function (schedule) {
                return getTimeTemplate(schedule, true);
            },
            time: function (schedule) {
                return getTimeTemplate(schedule, false);
            }
        }
    });

    // ===== Event handlers =====
    cal.on({
        'clickMore': function (e) { /* optional */ },
        'clickSchedule': function (ev) {
            var s = ev.schedule;

            // لو الحدث ليه دعوة للمستخدم الحالي (اختياري: تحقق من extendedProps)
            // هنا هنظهر Toast بسيط
            const container = document.createElement('div');
            container.className = 'position-fixed top-0 end-0 p-3';
            container.style.zIndex = 1080;
            container.innerHTML = `
    <div class="toast show">
      <div class="toast-header">
        <strong class="me-auto">Respond</strong>
        <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
      </div>
      <div class="toast-body d-flex align-items-center gap-2">
        <span class="text-truncate" style="max-width:200px">${(s.title || 'Event')}</span>
        <button class="btn btn-sm btn-success">Accept</button>
        <button class="btn btn-sm btn-outline-danger">Decline</button>
      </div>
    </div>`;
            document.body.appendChild(container);

            const btnAccept = container.querySelector('.btn-success');
            const btnDecline = container.querySelector('.btn-outline-danger');

            btnAccept.onclick = async () => {
                try { await respondEventOnApi(s.id, 'accepted'); }
                catch (e) { console.warn(e); }
                container.remove();
                if (typeof setSchedules === 'function') setSchedules();
            };
            btnDecline.onclick = async () => {
                try { await respondEventOnApi(s.id, 'declined'); }
                catch (e) { console.warn(e); }
                container.remove();
                if (typeof setSchedules === 'function') setSchedules();
            };
            /* detail popup الافتراضي هيشتغل */
},
        'clickDayname': function (date) { /* optional */ },

        'beforeCreateSchedule': async function (e) {
            // e: { title, isAllDay, start, end, location, calendarId, raw }
            // افتح المودال بدلاً من POST المباشر
            //$('#evt-id').value = '';
            //$('#evt-title').value = e.title || '';
            //$('#evt-start').value = toLocal(e.start?.toDate ? e.start.toDate() : e.start);
            //$('#evt-end').value = toLocal(e.end?.toDate ? e.end.toDate() : e.end);
            //$('#evt-location').value = e.location || '';
            //$('#evt-att').value = '';
            //$('#evt-del').style.display = 'none';
            //mApi?.show();

            //// خزن سياق لو حبيت تستخدمه لاحقًا
            //window.__evt_ctx__ = { mode: 'create' };
            try {
                const dto = {
                    title: e.title,
                    startUtc: e.start?.toDate ? e.start.toDate().toISOString() : new Date(e.start).toISOString(),
                    endUtc: e.end?.toDate ? e.end.toDate().toISOString() : new Date(e.end).toISOString(),
                    location: e.location || null,
                    attendeesUserIds: [] // ممكن تربطها لاحقاً من مودال خاص
                };
                const { eventId } = await createEventOnApi(dto);
                cal.createSchedules([{
                    ...e,
                    id: String(eventId),
                    calendarId: (e.calendarId || CalendarList[0]?.id || '1') + '',
                    category: e.isAllDay ? 'allday' : 'time',
                    color: CalendarList[0]?.color,
                    bgColor: CalendarList[0]?.bgColor,
                    dragBgColor: CalendarList[0]?.dragBgColor,
                    borderColor: CalendarList[0]?.borderColor,
                    state: 'Busy'
                }]);
            } catch (err) {
                console.warn('[calendar] create failed:', err);
            }
        },

        'beforeUpdateSchedule': async function (e) {
            // e: { schedule, changes }
            var schedule = e.schedule;
            var changes = e.changes || {};

            try {
                // جهّز DTO للتحديث
                const dto = {
                    id: schedule.id,
                    title: changes.title ?? schedule.title,
                    startUtc: (changes.start ? new Date(changes.start).toISOString() : (schedule.start?.toDate ? schedule.start.toDate().toISOString() : new Date(schedule.start).toISOString())),
                    endUtc: (changes.end ? new Date(changes.end).toISOString() : (schedule.end ? (schedule.end?.toDate ? schedule.end.toDate().toISOString() : new Date(schedule.end).toISOString()) : null)),
                    location: changes.location ?? schedule.location ?? null
                };
                await updateEventOnApi(dto);

                cal.updateSchedule(schedule.id, schedule.calendarId, changes);
                refreshScheduleVisibility();
            } catch (err) {
                console.warn('[calendar] update failed:', err);
            }
        },

        'beforeDeleteSchedule': async function (e) {
            try {
                await deleteEventOnApi(e.schedule.id);
                cal.deleteSchedule(e.schedule.id, e.schedule.calendarId);
            } catch (err) {
                console.warn('[calendar] delete failed:', err);
            }
        },

        'afterRenderSchedule': function (e) { /* optional */ },

        'clickTimezonesCollapseBtn': function (timezonesCollapsed) {
            if (timezonesCollapsed) {
                cal.setTheme({ 'week.daygridLeft.width': '77px', 'week.timegridLeft.width': '77px' });
            } else {
                cal.setTheme({ 'week.daygridLeft.width': '60px', 'week.timegridLeft.width': '60px' });
            }
            return true;
        }
    });

    // ===== templates =====
    function getTimeTemplate(schedule, isAllDay) {
        var html = [];
        var start = moment(schedule.start.toUTCString ? schedule.start.toUTCString() : schedule.start);

        if (!isAllDay) {
            html.push('<strong>' + start.format('HH:mm') + '</strong> ');
        }
        if (schedule.isPrivate) {
            html.push('<span class="calendar-font-icon ic-lock-b"></span> Private');
        } else {
            if (schedule.isReadOnly) {
                html.push('<span class="calendar-font-icon ic-readonly-b"></span>');
            } else if (schedule.recurrenceRule) {
                html.push('<span class="calendar-font-icon ic-repeat-b"></span>');
            } else if (schedule.attendees && schedule.attendees.length) {
                html.push('<span class="calendar-font-icon ic-user-b"></span>');
            } else if (schedule.location) {
                html.push('<span class="calendar-font-icon ic-location-b"></span>');
            }
            html.push(' ' + esc(schedule.title));
        }
        return html.join('');
    }

    // ===== UI menus (كما هي) =====
    function onClickMenu(e) {
        var target = $(e.target).closest('a[role="menuitem"]')[0];
        var action = getDataAction(target);
        var options = cal.getOptions();
        var viewName = '';

        switch (action) {
            case 'toggle-daily': viewName = 'day'; break;
            case 'toggle-weekly': viewName = 'week'; break;
            case 'toggle-monthly': options.month.visibleWeeksCount = 0; viewName = 'month'; break;
            case 'toggle-weeks2': options.month.visibleWeeksCount = 2; viewName = 'month'; break;
            case 'toggle-weeks3': options.month.visibleWeeksCount = 3; viewName = 'month'; break;
            case 'toggle-narrow-weekend':
                options.month.narrowWeekend = !options.month.narrowWeekend;
                options.week.narrowWeekend = !options.week.narrowWeekend;
                viewName = cal.getViewName();
                target.querySelector('input').checked = options.month.narrowWeekend;
                break;
            case 'toggle-start-day-1':
                options.month.startDayOfWeek = options.month.startDayOfWeek ? 0 : 1;
                options.week.startDayOfWeek = options.week.startDayOfWeek ? 0 : 1;
                viewName = cal.getViewName();
                target.querySelector('input').checked = options.month.startDayOfWeek;
                break;
            case 'toggle-workweek':
                options.month.workweek = !options.month.workweek;
                options.week.workweek = !options.week.workweek;
                viewName = cal.getViewName();
                target.querySelector('input').checked = !options.month.workweek;
                break;
            default: break;
        }

        cal.setOptions(options, true);
        cal.changeView(viewName, true);

        setDropdownCalendarType();
        setRenderRangeText();
        setSchedules(); // ← API
    }

    function onClickNavi(e) {
        var action = getDataAction(e.target);
        switch (action) {
            case 'move-prev': cal.prev(); break;
            case 'move-next': cal.next(); break;
            case 'move-today': cal.today(); break;
            default: return;
        }
        setRenderRangeText();
        setSchedules(); // ← API
    }

    function onNewSchedule() {
        // زر "Save" في مودال Skote الأصلي
        var title = $('#new-schedule-title').val();
        var location = $('#new-schedule-location').val();
        var isAllDay = document.getElementById('new-schedule-allday').checked;
        var start = datePicker.getStartDate();
        var end = datePicker.getEndDate();
        var calendar = selectedCalendar ? selectedCalendar : CalendarList[0];

        if (!title) return;

        // نرسل للسيرفر
        createEventOnApi({
            title: title,
            startUtc: start?.toISOString?.() ?? new Date(start).toISOString(),
            endUtc: end?.toISOString?.() ?? new Date(end).toISOString(),
            location: location || null,
            attendeesUserIds: []
        }).then(({ eventId }) => {
            cal.createSchedules([{
                id: String(eventId),
                calendarId: calendar.id,
                title: title,
                isAllDay: isAllDay,
                start: start,
                end: end,
                category: isAllDay ? 'allday' : 'time',
                dueDateClass: '',
                color: calendar.color,
                bgColor: calendar.bgColor,
                dragBgColor: calendar.bgColor,
                borderColor: calendar.borderColor,
                location: location,
                state: 'Busy'
            }]);
            $('#modal-new-schedule').modal('hide');
        }).catch(console.warn);
    }

    function onChangeNewScheduleCalendar(e) {
        var target = $(e.target).closest('a[role="menuitem"]')[0];
        var calendarId = getDataAction(target);
        changeNewScheduleCalendar(calendarId);
    }

    function changeNewScheduleCalendar(calendarId) {
        var calendarNameElement = document.getElementById('calendarName');
        var calendar = findCalendar(calendarId);
        var html = [];

        html.push('<span class="calendar-bar" style="background-color: ' + calendar.bgColor + '; border-color:' + calendar.borderColor + ';"></span>');
        html.push('<span class="calendar-name">' + calendar.name + '</span>');
        calendarNameElement.innerHTML = html.join('');
        selectedCalendar = calendar;
    }

    // ===== أهم تعديل: بدل الـ generator → API =====
    async function setSchedules() {
        try {
            cal.clear();
            const schedules = await fetchSchedules(cal.getDateRangeStart(), cal.getDateRangeEnd());
            cal.createSchedules(schedules);
            refreshScheduleVisibility();
        } catch (e) {
            console.warn('[calendar] load failed:', e);
        }
    }

    function onChangeCalendars(e) {
        var calendarId = e.target.value;
        var checked = e.target.checked;
        var viewAll = document.querySelector('.lnb-calendars-item input');
        var calendarElements = Array.prototype.slice.call(document.querySelectorAll('#calendarList input'));
        var allCheckedCalendars = true;

        if (calendarId === 'all') {
            allCheckedCalendars = checked;
            calendarElements.forEach(function (input) {
                var span = input.parentNode;
                input.checked = checked;
                span.style.backgroundColor = checked ? span.style.borderColor : 'transparent';
            });
            CalendarList.forEach(function (calendar) { calendar.checked = checked; });
        } else {
            findCalendar(calendarId).checked = checked;
            allCheckedCalendars = calendarElements.every(function (input) { return input.checked; });
            viewAll.checked = allCheckedCalendars;
        }
        refreshScheduleVisibility();
    }

    function refreshScheduleVisibility() {
        var calendarElements = Array.prototype.slice.call(document.querySelectorAll('#calendarList input'));
        CalendarList.forEach(function (calendar) {
            cal.toggleSchedules(calendar.id, !calendar.checked, false);
        });
        cal.render(true);
        calendarElements.forEach(function (input) {
            var span = input.nextElementSibling;
            span.style.backgroundColor = input.checked ? span.style.borderColor : 'transparent';
        });
    }

    function setDropdownCalendarType() {
        var calendarTypeName = document.getElementById('calendarTypeName');
        var calendarTypeIcon = document.getElementById('calendarTypeIcon');
        var options = cal.getOptions();
        var type = cal.getViewName();
        var iconClassName;

        if (type === 'day') { type = 'Daily'; iconClassName = 'calendar-icon ic_view_day'; }
        else if (type === 'week') { type = 'Weekly'; iconClassName = 'calendar-icon ic_view_week'; }
        else if (options.month.visibleWeeksCount === 2) { type = '2 weeks'; iconClassName = 'calendar-icon ic_view_week'; }
        else if (options.month.visibleWeeksCount === 3) { type = '3 weeks'; iconClassName = 'calendar-icon ic_view_week'; }
        else { type = 'Monthly'; iconClassName = 'calendar-icon ic_view_month'; }

        calendarTypeName.innerHTML = type;
        calendarTypeIcon.className = iconClassName;
    }

    function setRenderRangeText() {
        var renderRange = document.getElementById('renderRange');
        var options = cal.getOptions();
        var viewName = cal.getViewName();
        var html = [];
        if (viewName === 'day') {
            html.push(moment(cal.getDate().getTime()).format('YYYY.MM.DD'));
        } else if (viewName === 'month' && (!options.month.visibleWeeksCount || options.month.visibleWeeksCount > 4)) {
            html.push(moment(cal.getDate().getTime()).format('YYYY.MM'));
        } else {
            html.push(moment(cal.getDateRangeStart().getTime()).format('YYYY.MM.DD'));
            html.push(' ~ ');
            html.push(moment(cal.getDateRangeEnd().getTime()).format(' MM.DD'));
        }
        renderRange.innerHTML = html.join('');
    }

    function setEventListener() {
        $('#menu-navi').on('click', onClickNavi);
        $('.dropdown-menu a[role="menuitem"]').on('click', onClickMenu);
        $('#lnb-calendars').on('change', onChangeCalendars);

        $('#btn-save-schedule').on('click', onNewSchedule);
        $('#btn-new-schedule').on('click', function (ev) {
            const start = cal.getDate() || new Date();
            if (useCreationPopup) {
                cal.openCreationPopup({ start, end: moment(start).add(1, 'hours').toDate() });
            }
        });

        $('#dropdownMenu-calendars-list').on('click', onChangeNewScheduleCalendar);
        window.addEventListener('resize', resizeThrottled);
    }

    function getDataAction(target) {
        return target.dataset ? target.dataset.action : target.getAttribute('data-action');
    }

    // ==== Form actions (create/update/delete) ====
    // مهام API اللي عندك أصلاً:
  
    // submit (حفظ)
    
    resizeThrottled = tui.util.throttle(function () { cal.render(); }, 50);
    window.cal = cal;

    setDropdownCalendarType();
    setRenderRangeText();
    setSchedules();        // ← أول تحميل من الـAPI
    setEventListener();

    // ===== ربط SignalR (اختياري لو شغّال عندك) =====
    (async function attachRealtime() {
        try {
            if (!window.SignalR || !window.TokenProvider) return;
            const notify = await window.SignalR.get('notify', { tokenFactory: () => window.TokenProvider.getToken() });
            notify.on('Notify', (p) => {
                if (p?.kind?.startsWith?.('calendar-') || p?.meta?.eventId) setSchedules();
            });
            notify.on('ReceiveNotification', (p) => {
                if (p?.kind?.startsWith?.('calendar-') || p?.meta?.eventId) setSchedules();
            });
            await notify.invoke('JoinPersonal').catch(() => { });
        } catch (e) { /* ignore */ }
    })();

})(window, tui.Calendar);

// ==== قائمة الكالندرز (زي ما هي) ====
(function () {
    var calendarList = document.getElementById('calendarList');
    if (!calendarList) return;
    var html = [];
    CalendarList.forEach(function (calendar) {
        html.push('<div class="lnb-calendars-item"><label>' +
            '<input type="checkbox" class="tui-full-calendar-checkbox-round" value="' + calendar.id + '" checked>' +
            '<span style="border-color: ' + calendar.borderColor + '; background-color: ' + calendar.borderColor + ';"></span>' +
            '<span>' + calendar.name + '</span>' +
            '</label></div>'
        );
    });
    calendarList.innerHTML = html.join('\n');
})();
