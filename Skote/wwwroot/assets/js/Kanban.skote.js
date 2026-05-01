(function () {
    "use strict";

    const boardId = document.querySelector('[data-board]')?.getAttribute('data-board');
    if (!boardId) { console.warn('[kanban] missing boardId'); return; }

    const lists = {
        ToDo: document.getElementById('todo-list'),
        InProgress: document.getElementById('inprogress-list'),
        Done: document.getElementById('done-list'),
        Blocked: null // أضفه لاحقًا لو حبيت
    };

    const esc = s => String(s ?? '').replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
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
        
        // 5) توافق قديم
        if (window.__realtimeBase) {
            return String(window.__realtimeBase).replace(/\/+$/, '');
        }
        // 6) آخر حل: N/A عشان تعرف بسرعة إنه مفيش قيمة صحيحة
        return 'N/A';
    }
    function renderTask(t) {
        const li = document.createElement('li');
        li.className = 'list-group-item task-card d-flex flex-column';
        li.dataset.id = t.id;
        li.innerHTML = `
      <div class="d-flex justify-content-between align-items-center">
        <span class="fw-600">${esc(t.title)}</span>
        <span class="badge bg-${badgeByPriority(t.priority)}">${esc(t.priority || 'Medium')}</span>
      </div>
      ${t.assignedToUserId ? `<small class="text-muted mt-1"><i class="bx bx-user"></i> ${esc(t.assignedToUserId)}</small>` : ``}
      ${t.dueDateUtc ? `<small class="text-muted"><i class="bx bx-time"></i> ${new Date(t.dueDateUtc).toLocaleString()}</small>` : ``}
    `;
        li.addEventListener('dblclick', () => {
            window.openTaskEdit && window.openTaskEdit(t);
        });
        return li;
    }

    function badgeByPriority(p) {
        const x = (p || '').toLowerCase();
        if (x === 'critical') return 'danger';
        if (x === 'high') return 'warning';
        if (x === 'low') return 'info';
        return 'secondary';
    }

    async function apiGetTasks() {
        
        const t = await (window.TokenProvider?.getToken?.() ?? '');
        const res = await fetch(`https://localhost:7106/kanban/boards/${boardId}/tasks`, {
            headers: t ? { Authorization: 'Bearer ' + t } : {},
            credentials: 'include'
        });
        return res.json();
    }

    async function apiMove(taskId, newStatus) {
        const base = () => {
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

        };
        const t = await (window.TokenProvider?.getToken?.() ?? '');
        await fetch(`${base}/kanban/tasks/move`, {
            method: 'POST', credentials: 'include',
            headers: { 'Content-Type': 'application/json', ...(t ? { Authorization: 'Bearer ' + t } : {}) },
            body: JSON.stringify({ taskId, newStatus })
        });
    }

    async function loadBoard() {
        const data = await apiGetTasks();
        // فضّي القوائم
        Object.values(lists).forEach(ul => ul && (ul.innerHTML = ''));
        (data || []).forEach(t => {
            const target = lists[t.status] || lists.ToDo;
             target?.appendChild(renderTask(t));
        });
    }

    // Sortable
    ['todo-list', 'inprogress-list', 'done-list'].forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;
        new Sortable(el, {
            group: 'kanban', animation: 150,
            onAdd: async (evt) => {
                const taskId = evt.item.dataset.id;
                const newStatus = id === 'todo-list' ? 'ToDo' : id === 'inprogress-list' ? 'InProgress' : 'Done';
                try { await apiMove(taskId, newStatus); }
                catch (e) { console.warn(e); await loadBoard(); } // رجّع الحالة لو فشل
            }
        });
    });

    // SignalR
    (async function attachRealtime() {
        try {
            const kanban = await SignalR.get('kanban', { tokenFactory: () => TokenProvider.getToken() });
            await kanban.invoke('JoinBoard', boardId);
            kanban.on('TaskCreated', (t) => { if (t.boardId !== boardId) return; loadBoard(); });
            kanban.on('TaskMoved', (p) => { if (p.boardId !== boardId) return; loadBoard(); });
            kanban.on('TaskUpdated', () => loadBoard());
            kanban.on('TaskDeleted', () => loadBoard());
        } catch (e) { console.warn('[kanban] realtime disabled', e); }
    })();

    // أول تحميل
    loadBoard().catch(console.error);
   
    window.loadKanbanBoard = loadBoard;
})();