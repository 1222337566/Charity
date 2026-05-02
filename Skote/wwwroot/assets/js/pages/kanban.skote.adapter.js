(function () {
    "use strict";
    let drake;          // علشان ما نكررش التهيئة
    let lastPos = null; // لتتبّع موقع الكارت قبل السحب (rollback)
    // ========= Helpers =========
    function apiBase() {
        // 1) Override صريح
        if (typeof window.apiBase === 'function') {
            const v = window.apiBase();
            if (v) return String(v).replace(/\/+$/, '');
        }
        // 2) data-api-base من الصفحة
        const el = document.querySelector('[data-api-base]');
        if (el?.dataset?.apiBase) {
            return String(el.dataset.apiBase).replace(/\/+$/, '');
        }
        // 3) من __APP__ المحقون من السيرفر
        if (window.__APP__?.realtimeBase) {
            return String(window.__APP__.realtimeBase).replace(/\/+$/, '');
        }
        // 4) استنتاج من أي Hub URL (kanban > notify > chat)
        const hubs = window.__SIGNALR_HUBS__ || {};
        const pick = hubs.kanban || hubs.notify || hubs.chat;
        //if (pick) {
        //    try { return new URL(pick, location.origin).origin; } catch { }
        //}
        // 5) توافق قديم
        if (window.__realtimeBase) {
            return String(window.__realtimeBase).replace(/\/+$/, '');
        }
        // 6) آخر حل: N/A عشان تعرف بسرعة إنه مفيش قيمة صحيحة
        return 'N/A';
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
        return 'warning'; // medium
    };
    function currentBoardId() {
        const el = document.querySelector('[data-board]');
        if (el?.dataset?.board) return el.dataset.board;
        const u = new URL(location.href);
        return u.searchParams.get('board') || '';
    }

    // ========= أعمدة Skote =========
    const LISTS = {
        ToDo: document.getElementById('upcoming-task'),
        InProgress: document.getElementById('inprogress-task'),
        Done: document.getElementById('complete-task')
    };

    // ========= API =========
    async function apiGetTasks(boardId) {
        const base = apiBase();
        const t = await TokenProvider.getToken();
        const res = await fetch(`${base}/kanban/boards/${boardId}/tasks`, {
            credentials: 'include',
            headers: t ? { Authorization: 'Bearer ' + t } : {}
        });
        if (!res.ok) throw new Error('GET tasks failed: ' + res.status);
        const data = await res.json();
        return (data || []).map(x => ({
            id: x.id ?? x.Id,
            boardId: x.boardId ?? x.BoardId,
            title: x.title ?? x.Title,
            description: x.description ?? x.Description,
            status: normStatus(x.status ?? x.Status),
            priority: x.priority ?? x.Priority ?? 'Medium',
            assignedToUserId: x.assignedToUserId ?? x.AssignedToUserId ?? null,
            dueDateUtc: x.dueDateUtc ?? x.DueDateUtc ?? null
        }));
    }
    async function apiCreateTask(dto) {
        const base = apiBase();
        const t = await TokenProvider.getToken();
        const res = await fetch(`${base}/kanban/tasks`, {
            method: 'POST', credentials: 'include',
            headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
            body: JSON.stringify(dto)
        });
        if (!res.ok) throw new Error('POST task failed: ' + res.status);
        return res.json();
    }
    async function apiMoveTask(taskId, newStatus) {
        const base = apiBase();
        const t = await TokenProvider.getToken();
        const res = await fetch(`${base}/kanban/tasks/move`, {
            method: 'POST', credentials: 'include',
            headers: { 'Content-Type': 'application/json', Authorization: 'Bearer ' + t },
            body: JSON.stringify({ taskId, newStatus })
        });
        if (!res.ok) throw new Error('MOVE failed: ' + res.status);
        return res.json();
    }

    // ========= Render بطاقة Skote =========
    function dueClass(dt) {
        if (!dt) return '';
        const d = new Date(dt), now = new Date();
        const diff = (d - now) / 86400000; // أيام
        if (diff < 0) return 'text-danger';       // متأخر
        if (diff <= 2) return 'text-warning';     // قريب
        return 'text-muted';
    }
    function renderCard(t) {
        const statusBadge = {
            ToDo: 'badge-soft-secondary',
            InProgress: 'badge-soft-primary',
            Done: 'badge-soft-success'
        }[t.status] || 'badge-soft-secondary';

        const card = document.createElement('div');
        card.className = 'card task-box';
        card.id = t.id;
        card.dataset.taskId = t.id;

        card.innerHTML = `
      <div class="card-body">
        <div class="dropdown float-end">
          <a href="#" class="dropdown-toggle arrow-none" data-bs-toggle="dropdown">
            <i class="mdi mdi-dots-vertical m-0 text-muted h5"></i>
          </a>
          <div class="dropdown-menu dropdown-menu-end">
            <a class="dropdown-item edittask-details" href="#" data-bs-toggle="modal" data-bs-target=".bs-example-modal-lg" data-id="#${t.id}">Edit</a>
            <a class="dropdown-item deletetask" href="#" data-id="#${t.id}">Delete</a>
          </div>
        </div>
        <div class="float-end ms-2">
          <span class="badge rounded-pill font-size-12 ${statusBadge}" id="task-status">${t.status}</span>
        </div>
        <div>
          <h5 class="font-size-15"><a href="javascript:void(0)" class="text-dark" id="task-name">${esc(t.title || 'Task')}</a></h5>
          ${t.dueDateUtc ? `<p class="text-muted mb-2 ${dueClass(t.dueDateUtc)}" id="task-date">${new Date(t.dueDateUtc).toLocaleString()}</p>` : `<p class="text-muted mb-2" id="task-date"></p>`}
        </div>
        ${t.description ? `<ul class="ps-3 mb-4 text-muted" id="task-desc"><li class="py-1">${esc(t.description)}</li></ul>` : ``}
        <div class="avatar-group float-start task-assigne"></div>
        <div class="text-end">
          <span class="badge bg-${priBadge(t.priority)}">${esc(t.priority)}</span>
        </div>
      </div>
    `;
        card.addEventListener('dblclick', () => {
            if (window.openTaskEdit) window.openTaskEdit(t);
        });
        card.querySelector('#task-name')?.setAttribute('title', t.description || '');
        
        return card;
    }
    const WIP = { InProgress: 5 }; // غيّر الحد حسب رغبتك
    function applyWipWarning() {
        const col = LISTS.InProgress;
        if (!col) return;
        const n = col.querySelectorAll('.task-box').length;
        col.classList.toggle('border', n > WIP.InProgress);
        col.classList.toggle('border-3', n > WIP.InProgress);
        col.classList.toggle('border-danger', n > WIP.InProgress);
    }
    function listsClear() { Object.values(LISTS).forEach(col => { if (col) col.innerHTML = ''; }); }
    function appendToColumn(task) { (LISTS[task.status] || LISTS.ToDo)?.appendChild(renderCard(task)); }

    // ========= Load & Drag =========
    async function loadBoard() {
        const boardId = currentBoardId();
        if (!boardId) { console.warn('[kanban] no board id'); return; }
        const tasks = await apiGetTasks(boardId);
        listsClear();

        tasks.forEach(appendToColumn);
        applyWipWarning();
    }

    function wireDrag() {

        if (drake && drake.containers?.length) return;

        const todo = document.getElementById('upcoming-task');
        const prog = document.getElementById('inprogress-task');
        const done = document.getElementById('complete-task');

        if (!todo || !prog || !done) {
            console.warn('[kanban] columns not found for dragula');
            return;
        }

        drake = dragula([todo, prog, done], {
            direction: 'vertical',
            copy: false,
            revertOnSpill: true,
            removeOnSpill: false,
            // تجاهُل السحب فوق عناصر تفاعلية
            invalid: function (el, handle) {
                const tag = (handle?.tagName || '').toLowerCase();
                if (['a', 'button', 'input', 'textarea', 'select', 'label', 'i'].includes(tag)) return true;
                return handle?.closest?.('.dropdown-menu') ? true : false;
            },
            // السماح بالإسقاط فقط في الأعمدة الثلاثة
            accepts: function (el, target) {
                return [todo, prog, done].includes(target);
            },
            mirrorContainer: document.body
        });

        // التقط موقع الكارت عند بدء السحب (لـ rollback)
        drake.on('drag', (el, source) => {
            lastPos = { el, source, next: el.nextSibling };
        });

        // عند الإسقاط: حدّث الحالة عبر الـAPI
        drake.on('drop', async (el, target/*, source, sibling*/) => {
            try {
                const id = el?.dataset?.taskId || el?.id;
                let newStatus = 'ToDo';
                if (target === prog) newStatus = 'InProgress';
                else if (target === done) newStatus = 'Done';

                await apiMoveTask(id, newStatus);
                // (اختياري) لو عايز refresh للتأكيد:
                // await loadBoard();
            } catch (e) {
                console.error('[kanban] move failed', e);
                // رجوع محلي سريع لمكانه السابق
                try { lastPos?.source?.insertBefore(lastPos.el, lastPos.next); } catch { }
                // (اختياري) مزامنة كاملة من السيرفر:
                // await loadBoard();
            }
        });
        //if (!window.dragula) { console.warn('[kanban] dragula missing'); return; }
        //const cols = [LISTS.ToDo, LISTS.InProgress, LISTS.Done].filter(Boolean);
        //const drake = dragula(cols);
        //drake.on('drop', async (el, target/*, source, sibling*/) => {
        //    try {
        //        const id = el?.dataset?.taskId || el?.id;
        //        let newStatus = 'ToDo';
        //        if (target === LISTS.InProgress) newStatus = 'InProgress';
        //        else if (target === LISTS.Done) newStatus = 'Done';
        //        await apiMoveTask(id, newStatus);
        //    } catch (e) {
        //        console.error('[kanban] move failed', e);
        //        await loadBoard(); // rollback من السيرفر
        //    }
        //});
    }

    // ========= SignalR =========
    const debounce = (fn, ms = 200) => { let t; return (...a) => { clearTimeout(t); t = setTimeout(() => fn(...a), ms); }; };
    const safeReload = debounce(() => loadBoard().catch(console.error), 250);
    async function wireSignalR() {
        try {
            const hub = await SignalR.get('kanban', { tokenFactory: () => TokenProvider.getToken() });
            const boardId = currentBoardId();
            try { await hub.invoke('JoinBoard', boardId); } catch { }
            const refresh = () => loadBoard().catch(console.error);
            hub.on('TaskCreated', safeReload);
            hub.on('TaskMoved', safeReload);
            hub.on('TaskUpdated', safeReload);
            hub.on('TaskDeleted', safeReload);

          
        } catch (e) {
            console.warn('[kanban] signalr not ready', e);
        }
    }

    // ========= New Task من مودال Skote =========
    function wireCreateFromModal() {
        const btn = document.getElementById('addtask'); // زر Skote
        if (!btn) return;

        btn.addEventListener('click', async (ev) => {
            ev.preventDefault();
            ev.stopImmediatePropagation();

            try {
                const boardId = currentBoardId();
                const title = document.getElementById('taskname')?.value?.trim();
                const desc = document.getElementById('taskdesc')?.value?.trim() || '';
                const statusSel = document.getElementById('TaskStatus');
                const status = normStatus(statusSel?.value || 'ToDo');
                const priority = 'Medium'; // لو عندك عنصر للأولوية: استخرجه بدل الثابت

                if (!title) { console.warn('title required'); return; }

                await apiCreateTask({ boardId, title, description: desc, priority, status });

                // اقفل المودال (حسب ID المودال في Skote)
                const modal = document.getElementById('modalForm') || document.querySelector('.bs-example-modal-lg');
                if (modal && window.bootstrap) {
                    const m = bootstrap.Modal.getInstance(modal) || new bootstrap.Modal(modal);
                    // فضي الفوكس قبل الإغلاق (لمنع تحذير aria-hidden)
                    document.activeElement && document.activeElement.blur();
                    // اقفل المودال (حسب ID أو الكلاس)
                    const modal = document.getElementById('modalForm') || document.querySelector('.bs-example-modal-lg');
                    if (modal && window.bootstrap) {
                        const m = bootstrap.Modal.getInstance(modal) || new bootstrap.Modal(modal);
                        // مهم: فضي الفوكس قبل الإغلاق لتفادي تحذير aria-hidden
                        document.activeElement && document.activeElement.blur();
                        m.hide();
                        // (اختياري) رجِّع الفوكس لزر New Task
                        setTimeout(() => document.querySelector('[data-bs-target="#modalForm"], [data-bs-target=".bs-example-modal-lg"]')?.focus(), 0);
                    }
                    m.hide();

                }

                await loadBoard();
            } catch (e) {
                console.error('[kanban] create failed', e);
            }
        }, { capture: true });
    }

    // ========= Expose للديبج =========
    window.KanbanUI = { reload: loadBoard };

    // ========= Start =========
    document.addEventListener('DOMContentLoaded', () => {
        const tt = [].slice.call(document.querySelectorAll('[title]'));
        tt.forEach(el => new bootstrap.Tooltip(el));
    });
    document.addEventListener('DOMContentLoaded', async () => {
        await loadBoard();
        wireDrag();
        wireSignalR();
        wireCreateFromModal();
        console.log('[kanban] apiBase =', apiBase(), 'boardId =', currentBoardId());
    });

})();