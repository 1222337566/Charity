(function () {
    "use strict";
    const DEBUG = !!localStorage.KANBAN_DEBUG;
    function log(...a) { if (DEBUG) console.log('[kanban]', ...a); }

    async function ensureOk(res, fallback) {
        if (res.ok) return res;
        let msg = fallback || 'Request failed';
        try { const j = await res.json(); msg = j?.message || j?.error || msg; } catch { }
        window.toastr?.error?.(msg);
        throw new Error(msg);
    }
    function fmtRelative(dt) {
        if (!dt) return '';
        const d = new Date(dt), now = new Date(), m = (d - now) / 60000;
        if (m < -1440) return `${Math.ceil(Math.abs(m) / 1440)}d overdue`;
        if (m < 0) return `${Math.ceil(Math.abs(m))}m overdue`;
        if (m < 60) return `in ${Math.ceil(m)}m`;
        if (m < 1440) return `in ${Math.ceil(m / 60)}h`;
        return d.toLocaleString();
    }
    // ثبّت Prefix حسب API عندك (/kanban أو /api/kanban)
    const API_PREFIX = (window.__KANBAN_API_PREFIX__ || '/kanban').replace(/\/+$/, '');

    let drake;
    let lastPos = null;
    let cacheTasks = [];
    function escapeReg(s) { return s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); }
    function hi(text, q) {
        if (!q) return esc(text);
        return esc(text).replace(new RegExp(`(${escapeReg(q)})`, 'ig'), '<mark>$1</mark>');
    }
    // ========== Helpers ==========
    function apiBase() {
        if (typeof window.apiBase === 'function') {
            const v = window.apiBase(); if (v) return String(v).replace(/\/+$/, '');
        }
        const el = document.querySelector('[data-api-base]');
        if (el?.dataset?.apiBase) return String(el.dataset.apiBase).replace(/\/+$/, '');
        if (window.__APP__?.realtimeBase) return String(window.__APP__.realtimeBase).replace(/\/+$/, '');
        const hubs = window.__SIGNALR_HUBS__ || {};
        const pick = hubs.kanban || hubs.notify || hubs.chat;
       
        if (window.__realtimeBase) return String(window.__realtimeBase).replace(/\/+$/, '');
        throw new Error('[kanban] apiBase not found. Inject window.__APP__.realtimeBase or data-api-base.');
    }

    const esc = s => String(s ?? "").replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
    const normStatus = s => {
        const k = (s || '').toLowerCase();
        if (k === 'todo' || k === 'upcoming') return 'ToDo';
        if (k === 'inprogress' || k === 'in_progress') return 'InProgress';
        if (k === 'done' || k === 'complete' || k === 'completed') return 'Done';
        return 'ToDo';
    };
    const priBadge = p => {
        const k = (p || 'medium').toLowerCase();
        if (k === 'high') return 'danger';
        if (k === 'low') return 'secondary';
        return 'warning';
    };

    function currentBoardId() {
        const el = document.querySelector('[data-board]');
        if (el?.dataset?.board) return el.dataset.board;
        const u = new URL(location.href);
        return u.searchParams.get('board') || '';
    }

    function getLists() {
        let ToDo = document.getElementById('upcoming-task');
        let InProgress = document.getElementById('inprogress-task');
        let Done = document.getElementById('complete-task');

        if (!ToDo || !InProgress || !Done) {
            const all = Array.from(document.querySelectorAll('[data-status]'));
            ToDo = ToDo || all.find(x => /todo|upcoming/i.test(x.dataset.status || ''));
            InProgress = InProgress || all.find(x => /in[_-]?progress/i.test(x.dataset.status || ''));
            Done = Done || all.find(x => /done|complete(d)?/i.test(x.dataset.status || ''));
        }
        if (!ToDo || !InProgress || !Done) {
            const cols = Array.from(document.querySelectorAll('.kanban-column, .tasks, .tasks-list, .row > [class*="col"]'))
                .filter(e => e.querySelector('.task-box, .card, [data-task], [data-id]'));
            if (cols.length >= 3) { ToDo = ToDo || cols[0]; InProgress = InProgress || cols[1]; Done = Done || cols[2]; }
        }
        [ToDo, InProgress, Done].forEach(c => { if (c && !c.style.minHeight) c.style.minHeight = '220px'; });
        return { ToDo, InProgress, Done };
    }

    // ========== API ==========
    async function apiGetTasks(boardId) {
        const base = apiBase(); const t = await TokenProvider.getToken();
        const res = await fetch(`${base}${API_PREFIX}/boards/${boardId}/tasks`, {
            credentials: 'include',
            headers: t ? { Authorization: 'Bearer ' + t } : {}
        });
        await ensureOk(res, 'Load failed');
        return res.json();
    }

    async function apiCreateTask(dto) {
        const base = apiBase(); const t = await TokenProvider.getToken();
        const res = await fetch(`${base}${API_PREFIX}/tasks`, {
            method: 'POST', credentials: 'include',
            headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
            body: JSON.stringify(dto)
        });
        await ensureOk(res, 'Create failed');
        return res.json();
    }

    async function apiUpdateTask(dto) {
        const base = apiBase(); const t = await TokenProvider.getToken();
        const res = await fetch(`${base}${API_PREFIX}/tasks`, {
            method: 'PUT', credentials: 'include',
            headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
            body: JSON.stringify(dto)
        });
        await ensureOk(res, 'Update failed');
        return res.json();
    }

    /* ← لاحظ إضافة orderIndex كبراميتر */
    async function apiMoveTask(taskId, newStatus, orderIndex) {
        const base = apiBase(); const t = await TokenProvider.getToken();
        const res = await fetch(`${base}${API_PREFIX}/tasks/move`, {
            method: 'POST', credentials: 'include',
            headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
            body: JSON.stringify({ taskId, newStatus, orderIndex })
        });
        await ensureOk(res, 'Move failed');
        return res.json();
    }

    async function apiDeleteTask(taskId) {
        const base = apiBase(); const t = await TokenProvider.getToken();
        const res = await fetch(`${base}${API_PREFIX}/tasks/${encodeURIComponent(taskId)}`, {
            method: 'DELETE', credentials: 'include',
            headers: t ? { Authorization: 'Bearer ' + t } : {}
        });
        await ensureOk(res, 'Delete failed');
        return res.text();
    }

    // ========== Rendering ==========
    function dueClass(dt) {
        if (!dt) return '';
        const d = new Date(dt), now = new Date();
        const diff = (d - now) / 86400000;
        if (diff < 0) return 'text-danger';
        if (diff <= 2) return 'text-warning';
        return 'text-muted';
    }

    function renderCard(t) {
        const statusBadge = { ToDo: 'badge-soft-secondary', InProgress: 'badge-soft-primary', Done: 'badge-soft-success' }[t.status] || 'badge-soft-secondary';
        const card = document.createElement('div');
        card.className = 'card task-box';
        card.id = t.id; card.dataset.taskId = t.id;
        const { q } = (typeof getFilterValues === 'function' ? getFilterValues() : { q: '' });
        const titleHtml = hi(t.title || 'Task', q);
        card.innerHTML = `
      <div class="card-body">
        <div class="dropdown float-end">
          <a href="javascript:void(0)" class="dropdown-toggle arrow-none" data-bs-toggle="dropdown" aria-expanded="false">
            <i class="mdi mdi-dots-vertical m-0 text-muted h5"></i>
          </a>
          <div class="dropdown-menu dropdown-menu-end">
            <a class="dropdown-item edittask-details" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target=".bs-example-modal-lg" data-id="#${t.id}">Edit</a>
            <a class="dropdown-item move-to-todo"   href="javascript:void(0)">Move to ToDo</a>
            <a class="dropdown-item move-to-inprog" href="javascript:void(0)">Move to InProgress</a>
            <a class="dropdown-item move-to-done"   href="javascript:void(0)">Move to Done</a>
            <a class="dropdown-item deletetask"     href="javascript:void(0)" data-id="#${t.id}">Delete</a>
          </div>
        </div>

        <div class="float-end ms-2">
          <span class="badge rounded-pill font-size-12 ${statusBadge}" id="task-status">${t.status}</span>
        </div>

        <div>
          <h5 class="font-size-15">
            <a href="javascript:void(0)" class="text-dark" id="task-name">${titleHtml}</a>
            </h5>
         ${t.dueDateUtc
                ? `<p class="mb-2 ${dueClass(t.dueDateUtc)}" id="task-date"><i class="bx bx-time"></i> ${fmtRelative(t.dueDateUtc)}</p>`
                : `<p class="text-muted mb-2" id="task-date"></p>`}${t.dueDateUtc
                    ? `<p class="mb-2 ${dueClass(t.dueDateUtc)}" id="task-date"><i class="bx bx-time"></i> ${fmtRelative(t.dueDateUtc)}</p>`
                    : `<p class="text-muted mb-2" id="task-date"></p>`}
        </div>

        ${t.description ? `<ul class="ps-3 mb-4 text-muted" id="task-desc"><li class="py-1">${esc(t.description)}</li></ul>` : ``}

        <div class="avatar-group float-start task-assigne"></div>
        <div class="text-end">
          <span class="badge bg-${priBadge(t.priority)}">${esc(t.priority)}</span>
        </div>
      </div>
    `;

        // نقل سريع
        card.querySelector('.move-to-todo')?.addEventListener('click', e => { e.preventDefault(); apiMoveTask(t.id, 'ToDo').then(loadBoard).catch(console.error); });
        card.querySelector('.move-to-inprog')?.addEventListener('click', e => { e.preventDefault(); apiMoveTask(t.id, 'InProgress').then(loadBoard).catch(console.error); });
        card.querySelector('.move-to-done')?.addEventListener('click', e => { e.preventDefault(); apiMoveTask(t.id, 'Done').then(loadBoard).catch(console.error); });

        // حذف
        card.querySelector('.deletetask')?.addEventListener('click', async (e) => {
            e.preventDefault();
            if (!confirm('Delete this task?')) return;
            try { await apiDeleteTask(t.id); await loadBoard(); }
            catch (err) { console.error('[kanban] delete failed', err); window.toastr?.error?.('Delete failed'); }
        });

        // دبل كليك يفتح مودال (لو متاح)
        card.addEventListener('dblclick', () => { if (window.openTaskEdit) window.openTaskEdit(t); });

        // Tooltip
        card.querySelector('#task-name')?.setAttribute('title', t.description || '');
     
        // تعديل عنوان سريع (اختياري)
        const titleEl = card.querySelector('#task-name');
        titleEl.addEventListener('dblclick', () => {
            const old = t.title;
            const inp = document.createElement('input');
            inp.className = 'form-control form-control-sm'; inp.value = old;
            titleEl.replaceWith(inp); inp.focus();
            inp.addEventListener('keydown', async (e) => {
                if (e.key === 'Enter') {
                    const val = inp.value.trim();
                    try { if (val && val !== old) await apiUpdateTask({ taskId: t.id, title: val }); }
                    catch (err) { console.error('[kanban] inline update failed', err); }
                    finally { await loadBoard(); }
                }
                if (e.key === 'Escape') { loadBoard(); }
            });
        });

        return card;
    }

    const WIP = { InProgress: 5 };
    function applyWipWarning() {
        const { InProgress } = getLists();
        if (!InProgress) return;
        const n = InProgress.querySelectorAll('.task-box').length;
        InProgress.classList.toggle('border', n > WIP.InProgress);
        InProgress.classList.toggle('border-3', n > WIP.InProgress);
        InProgress.classList.toggle('border-danger', n > WIP.InProgress);
    }

    function listsClear() {
        const lists = getLists();
        Object.values(lists).forEach(col => { if (col) col.innerHTML = ''; });
    }

    function appendToColumn(task) {
        const lists = getLists();
        (lists[task.status] || lists.ToDo)?.appendChild(renderCard(task));
    }

    // ====== Counters (مع fallback ذكي) ======
    function getCounterEl(kind) {
        const byData = {
            ToDo: document.querySelector('[data-count="todo"]'),
            InProgress: document.querySelector('[data-count="inprogress"]'),
            Done: document.querySelector('[data-count="done"]')
        }[kind];
        if (byData) return byData;

        const byId = {
            ToDo: document.getElementById('count-todo'),
            InProgress: document.getElementById('count-inprogress'),
            Done: document.getElementById('count-done')
        }[kind];
        if (byId) return byId;

        try {
            const header = ({
                ToDo: document.querySelector('#upcoming-task')?.closest('.card')?.querySelector('h5'),
                InProgress: document.querySelector('#inprogress-task')?.closest('.card')?.querySelector('h5'),
                Done: document.querySelector('#complete-task')?.closest('.card')?.querySelector('h5'),
            })[kind];
            if (header) {
                let badge = header.querySelector('.badge');
                if (!badge) {
                    badge = document.createElement('span');
                    badge.className = 'badge bg-secondary align-middle ms-1';
                    badge.textContent = '0';
                    header.appendChild(badge);
                }
                return badge;
            }
        } catch { }

        return null;
    }

    function updateCounters() {
        const lists = getLists();
        const counts = {
            ToDo: lists.ToDo?.querySelectorAll('.task-box').length || 0,
            InProgress: lists.InProgress?.querySelectorAll('.task-box').length || 0,
            Done: lists.Done?.querySelectorAll('.task-box').length || 0
        };

        const elTodo = getCounterEl('ToDo');
        const elProg = getCounterEl('InProgress');
        const elDone = getCounterEl('Done');

        if (elTodo) { elTodo.textContent = counts.ToDo; elTodo.style.removeProperty('display'); }
        if (elProg) { elProg.textContent = counts.InProgress; elProg.style.removeProperty('display'); }
        if (elDone) { elDone.textContent = counts.Done; elDone.style.removeProperty('display'); }

        if (!elTodo || !elProg || !elDone) {
            console.warn('[kanban] counter elements missing', { elTodo: !!elTodo, elProg: !!elProg, elDone: !!elDone });
        }
    }

    function updateEmptyPlaceholders() {
        const lists = getLists();
        Object.entries(lists).forEach(([k, col]) => {
            if (!col) return;
            if (!col.querySelector('.task-box')) {
                const hint = document.createElement('div');
                hint.className = 'kanban-empty';
                hint.textContent = (k === 'ToDo' ? 'No ToDo items' : '') || (k === 'InProgress' ? 'Nothing in progress' : '') || (k === 'Done' ? 'No completed tasks yet' : '');
                col.appendChild(hint);
            }
        });
    }

    // ====== Filters ======
    function getFilterValues() {
        const txt = document.getElementById('kanban-filter');
        const sel = document.getElementById('kanban-status');
        return {
            q: (txt?.value || '').toLowerCase().trim(),
            st: (sel?.value || '').trim()
        };
    }

    function renderFiltered() {
        const { q, st } = getFilterValues();
        listsClear();
        (cacheTasks || [])
            .filter(t => (st ? t.status === st : true))
            .filter(t => (t.title || '').toLowerCase().includes(q))
            .forEach(appendToColumn);
        updateCounters();
        applyWipWarning();
        updateEmptyPlaceholders();
    }

    // ====== Load (بدون Skeleton) ======
    async function loadBoard() {
        const boardId = currentBoardId();
        if (!boardId) { console.warn('[kanban] no board id'); return; }

        listsClear(); // تفريغ سريع فقط

        try { cacheTasks = await apiGetTasks(boardId); }
        catch (e) { console.error('[kanban] apiGetTasks failed', e); return; }

        renderFiltered();
    }

    // ====== Dragula ======
    function wireDrag() {
        if (drake && drake.containers?.length) return;
        const { ToDo, InProgress, Done } = getLists();
        if (!ToDo || !InProgress || !Done) { console.warn('[kanban] columns not found for dragula'); return; }
        if (!window.dragula) { console.warn('[kanban] dragula library missing'); return; }

        drake = dragula([ToDo, InProgress, Done], {
            direction: 'vertical', copy: false, revertOnSpill: true, removeOnSpill: false,
            invalid: function (el, handle) {
                const tag = (handle?.tagName || '').toLowerCase();
                if (['a', 'button', 'input', 'textarea', 'select', 'label', 'i'].includes(tag)) return true;
                return handle?.closest?.('.dropdown-menu') ? true : false;
            },
            accepts: function (el, target) { return [ToDo, InProgress, Done].includes(target); },
            mirrorContainer: document.body
        });

        drake.on('drag', (el, source) => { lastPos = { el, source, next: el.nextSibling }; el.classList.add('opacity-75'); });
        drake.on('cancel', (el) => { el.classList.remove('opacity-75'); });

        drake.on('drop', async (el, target) => {
            el.classList.remove('opacity-75');
            try {
                const id = el?.dataset?.taskId || el?.id;
                let newStatus = 'ToDo';
                if (target === InProgress) newStatus = 'InProgress';
                else if (target === Done) newStatus = 'Done';
                const newIndex = Array.from(target.children).indexOf(el); // 👈
                await apiMoveTask(id, newStatus, newIndex);
            } catch (e) {
                console.error('[kanban] move failed', e);
                try { lastPos?.source?.insertBefore(lastPos.el, lastPos.next); } catch { }
                window.toastr?.error?.('Move failed');
            }
        });

        async function apiMoveTask(taskId, newStatus, orderIndex) {
            const base = apiBase(); const t = await TokenProvider.getToken();
            const res = await fetch(`${base}${API_PREFIX}/tasks/move`, {
                method: 'POST', credentials: 'include',
                headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
                body: JSON.stringify({ taskId, newStatus, orderIndex })
            });
            await ensureOk(res, 'Move failed');
            return res.json();
        }
    }

    // ====== SignalR (مرن) ======
    const _debounce = (fn, ms = 150) => { let t; return (...a) => { clearTimeout(t); t = setTimeout(() => fn(...a), ms); }; };
    const reRender = _debounce(() => renderFiltered(), 120);

    document.addEventListener('DOMContentLoaded', () => {
        const txt = document.getElementById('kanban-filter');
        const sel = document.getElementById('kanban-status');

        if (txt) txt.addEventListener('input', reRender);
        if (sel) sel.addEventListener('change', reRender);
    });
    // const debounce = (fn, ms = 200) => { let t; return (...a) => { clearTimeout(t); t = setTimeout(() => fn(...a), ms); }; };
    const safeReload = _debounce(() => loadBoard().catch(console.error), 250);

    function showConn(b) {
        const el = document.getElementById('kanban-connection-banner');
        if (el) el.style.setProperty('display', b ? 'block' : 'none');
    }

    async function wireSignalR() {
        try {
            const hub = await SignalR.get('kanban', { tokenFactory: () => TokenProvider.getToken() });
            const boardId = currentBoardId();
            try { await hub.invoke?.('JoinBoard', boardId); } catch { }
            const conn = hub?.connection || hub;
            if (typeof conn.onreconnecting === 'function') conn.onreconnecting(() => showConn(true));
            if (typeof conn.onreconnected === 'function') conn.onreconnected(() => showConn(false));
            if (typeof conn.onclose === 'function') conn.onclose(() => showConn(true));
            hub.on?.('TaskCreated', safeReload);
            hub.on?.('TaskMoved', safeReload);
            hub.on?.('TaskUpdated', safeReload);
            hub.on?.('TaskDeleted', safeReload);
        } catch (e) {
            console.warn('[kanban] signalr not ready', e);
            showConn(true);
        }
    }

    // ====== Modal Create ======
    function wireCreateFromModal() {
        const btn = document.getElementById('addtask'); if (!btn) return;
        btn.setAttribute('type', 'button');
        btn.addEventListener('click', async (ev) => {
            ev.preventDefault(); ev.stopImmediatePropagation();
            try {
                const boardId = currentBoardId();
                const title = document.getElementById('taskname')?.value?.trim();
                const desc = document.getElementById('taskdesc')?.value?.trim() || '';
                const statusSel = document.getElementById('TaskStatus');
                const status = normStatus(statusSel?.value || 'ToDo');
                const priority = 'Medium';
                if (!title) { console.warn('title required'); return; }
                await apiCreateTask({ boardId, title, description: desc, priority, status });

                const modal = document.querySelector('.bs-example-modal-lg') || document.getElementById('modalForm');
                if (modal && window.bootstrap) {
                    document.activeElement?.blur();
                    const m = bootstrap.Modal.getInstance(modal) || new bootstrap.Modal(modal);
                    m.hide();
                    setTimeout(() => document.querySelector('[data-bs-target=".bs-example-modal-lg"], [data-bs-target="#modalForm"]')?.focus(), 0);
                }
                await loadBoard();
            } catch (e) {
                console.error('[kanban] create failed', e);
                window.toastr?.error?.('Create failed');
            }
        }, { capture: true });

        const form = document.getElementById('task-form') || document.querySelector('form#taskForm') || document.querySelector('.task-form');
        if (form) form.addEventListener('submit', (e) => { e.preventDefault(); btn.click(); });
    }

    // منع تنقّل الروابط الفارغة/#
    document.addEventListener('DOMContentLoaded', () => {
        document.addEventListener('click', (e) => {
            const a = e.target.closest('a'); if (!a) return;
            const href = (a.getAttribute('href') || '').trim();
            if (href === '#' || href === '') e.preventDefault();
        });
    });

    // Expose
    window.KanbanUI = { reload: loadBoard };

    document.addEventListener('DOMContentLoaded', () => {
        try {
            const tt = [].slice.call(document.querySelectorAll('[title]'));
            tt.forEach(el => window.bootstrap && new bootstrap.Tooltip(el));
        } catch { }
    });

    document.addEventListener('DOMContentLoaded', async () => {
        try {
            await loadBoard();
            updateCounters(); // تأكيد تحديث العدادات بعد أول تحميل
            wireDrag();
            wireSignalR();
            wireCreateFromModal();
            console.log('[kanban] apiBase =', apiBase(), 'boardId =', currentBoardId(), 'prefix =', API_PREFIX);
        } catch (e) {
            console.error(e);
        }
    });

})();
