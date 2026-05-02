(function () {
    "use strict";

    // --- UI refs (مع حمايات)
    const ui = {
        root: document.getElementById('directory-root') || document,
        ou: document.getElementById('ouDn'),
        q: document.getElementById('query'),
        btnRefresh: document.getElementById('btn-refresh'),
        btnPromote: document.getElementById('btn-promote'),
        rows: document.getElementById('dir-rows'),
        chkAll: document.getElementById('chk-all'),
        info: document.getElementById('result-info'),
        prev: document.getElementById('btn-prev'),
        next: document.getElementById('btn-next')
    };

    const toast = {
        ok: (m) => (window.toastr?.success || alert)(m),
        err: (m) => (window.toastr?.error || alert)(m),
        info: (m) => (window.toastr?.info || alert)(m),
    };

    const state = { page: 1, take: 50, total: 0, data: [], selected: new Set() };

    const esc = s => String(s ?? '')
        .replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));

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
    // --- Auth header helper
    async function authHeaders(extra) {
        let headers = { 'Accept': 'application/json' };
        try {
            if (window.TokenProvider?.getToken) {
                const t = await window.TokenProvider.getToken();
                if (t) headers['Authorization'] = 'Bearer ' + t;
            }
        } catch { /* ignore */ }
        if (extra) headers = { ...headers, ...extra };
        return headers;
    }

    // --- Load data
    async function load() {
        if (!ui.rows) return;
        const params = new URLSearchParams();
        if (ui.q?.value) params.set('q', ui.q.value.trim());
        if (ui.ou?.value) params.set('ouDn', ui.ou.value.trim());
        params.set('page', String(state.page));
        params.set('take', String(state.take));

        try {
            const r = await fetch(`${apiBase()}/admin/ad/directory?${params.toString()}`, {
                headers: await authHeaders()
            });
            if (!r.ok) throw new Error(`HTTP ${r.status}`);
            const j = await r.json();
            state.total = Number(j.total || 0);
            state.data = Array.isArray(j.items) ? j.items : [];
            render();
        } catch (e) {
            console.error(e);
            toast.err('فشل تحميل البيانات');
        }
    }

    // --- DHCP Modal helpers
    function getModal() {
        const el = document.getElementById('dhcpModal');
        if (!el) { toast.err('Modal غير متوفر بالصفحة'); return null; }
        const modal = bootstrap.Modal.getOrCreateInstance(el);
        return { el, modal, body: document.getElementById('dhcpModalBody'), meta: document.getElementById('dhcpMeta') };
    }

    function renderSpinner(target, text) {
        if (!target) return;
        target.innerHTML = `
      <div class="d-flex align-items-center gap-2">
        <div class="spinner-border" role="status" aria-hidden="true"></div>
        <strong>${esc(text || 'جاري التحميل...')}</strong>
      </div>`;
    }

    function renderDhcpTable(rows, meta) {
        if (!Array.isArray(rows) || rows.length === 0) {
            return `<div class="alert alert-warning mb-0">لا توجد سجلات DHCP</div>`;
        }
        const th = `
      <thead class="table-light">
        <tr>
          <th>#</th><th>IP</th><th>MAC</th><th>Hostname</th>
          
        </tr>
      </thead>`;
        const tb = `<tbody>${rows.map((r, i) => `
        <tr>
          <td>${i + 1}</td>
          <td>${esc(r.ipAddress)}</td>
          <td>${esc(r.macAddress)}</td>
          <td>${esc(r.hostName)}</td>
       
        </tr>`).join('')
            }</tbody>`;

        const metaBar = meta
            ? `إجمالي: ${meta.total ?? rows.length} | آخر تحديث: ${esc(meta.refreshedAt || '')}`
            : `إجمالي: ${rows.length}`;

        return {
            html: `<div class="table-responsive"><table class="table table-sm table-hover">${th}${tb}</table></div>`,
            meta: metaBar
        };
    }

    async function openDhcpModal(adId) {
        const ctx = getModal();
        if (!ctx) return;
        const { modal, body, meta } = ctx;
        renderSpinner(body, 'جاري تحميل بيانات DHCP ...');
        if (meta) meta.textContent = '';

        try {
            const url = `${apiBase()}/admin/ad/devices/refresh/${encodeURIComponent(adId)}`;
            const r = await fetch(url, { headers: await authHeaders() });
            if (!r.ok) throw new Error(`HTTP ${r.status}`);
            const j = await r.json();

            const items = Array.isArray(j.items) ? j.items : (Array.isArray(j) ? j : []);
            const view = renderDhcpTable(items, j.meta);
            body.innerHTML = typeof view === 'string' ? view : view.html;
            if (meta && typeof view !== 'string') meta.textContent = view.meta;

            const title = document.getElementById('dhcpModalLabel');
            if (title) title.textContent = `DHCP Leases - ${adId}`;
            modal.show();
        } catch (e) {
            console.error(e);
            body.innerHTML = `<div class="alert alert-danger">تعذر تحميل بيانات DHCP</div>`;
            modal.show();
        }
    }

    // --- Render table
    function render() {
        if (!ui.rows) return;
        ui.rows.innerHTML = '';

        if (!state.data.length) {
            ui.rows.innerHTML = `<tr><td colspan="9" class="text-muted">لا توجد بيانات</td></tr>`;
        } else {
            for (const d of state.data) {
                const adId = String(d.adObjectId || d.id || '');
                const tr = document.createElement('tr');
                tr.setAttribute('data-id', adId); // مهم لتفويض الأحداث

                tr.innerHTML = `
          <td><input type="checkbox" class="chk-row form-check-input" data-id="${esc(adId)}"></td>
          <td>${esc(d.displayName || '')}</td>
          <td>${esc(d.upn || '')}</td>
          <td>${esc(d.email || '')}</td>
          <td>${esc(d.department || '')}</td>
          <td><code>${esc(adId)}</code></td>
          <td><small class="text-muted">${d.fetchedAtUtc ? esc(new Date(d.fetchedAtUtc).toLocaleString()) : '-'}</small></td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-info" data-action="devices" data-id="${esc(d.id)}">الأجهزة</button>
              <button class="btn btn-outline-primary" data-action="dhcp" data-id="${esc(d.id)}">DHCP</button>
            </div>
          </td>
          <td>
            ${(() => {
                        const raw = d.userWorkstations || '';
                        const arr = raw.split(/[;,]/).map(x => x.trim()).filter(Boolean);
                        const preview = arr.slice(0, 3).join(', ');
                        const more = arr.length > 3 ? ` +${arr.length - 3} more` : '';
                        return `<span title="${esc(raw)}">${esc(preview)}${esc(more)}</span>`;
                    })()}
          </td>
        `;
                ui.rows.appendChild(tr);
            }
        }

        const maxPage = Math.max(1, Math.ceil((state.total || 0) / state.take));
        if (ui.info) ui.info.textContent = `صفحة ${state.page} من ${maxPage} — إجمالي: ${state.total}`;
        if (ui.prev) ui.prev.disabled = state.page <= 1;
        if (ui.next) ui.next.disabled = state.page >= maxPage;
        if (ui.btnPromote) ui.btnPromote.disabled = state.selected.size === 0;
    }

    // --- Selection
    ui.rows?.addEventListener('change', (e) => {
        const cb = e.target.closest?.('.chk-row'); if (!cb) return;
        const id = cb.dataset.id;
        if (cb.checked) state.selected.add(id); else state.selected.delete(id);
        if (ui.btnPromote) ui.btnPromote.disabled = state.selected.size === 0;
    });

    ui.chkAll?.addEventListener('change', () => {
        const cbs = ui.rows?.querySelectorAll('.chk-row') || [];
        state.selected.clear();
        cbs.forEach(cb => { cb.checked = ui.chkAll.checked; if (cb.checked) state.selected.add(cb.dataset.id); });
        if (ui.btnPromote) ui.btnPromote.disabled = state.selected.size === 0;
    });

    // --- Refresh from LDAP
    ui.btnRefresh?.addEventListener('click', async () => {
        const params = new URLSearchParams();
        if (ui.q?.value) params.set('q', ui.q.value.trim());
        if (ui.ou?.value) params.set('ou', ui.ou.value.trim());
        params.set('take', '1000');

        try {
            const r = await fetch(`${apiBase()}/admin/ad/refresh?${params.toString()}`, {
                method: 'POST',
                headers: await authHeaders({ 'Content-Type': 'application/json' })
            });
            const j = await r.json().catch(() => ({}));
            if (r.ok) { toast.ok(`تم تحديث ${j.fetched || 0} من LDAP`); state.page = 1; await load(); }
            else { toast.err(j.message || 'Refresh failed'); }
        } catch (e) {
            console.error(e);
            toast.err('خطأ أثناء التحديث');
        }
    });

    // --- Promote
    ui.btnPromote?.addEventListener('click', async () => {
        const ids = Array.from(state.selected);
        if (!ids.length) return;
        try {
            const r = await fetch(`${apiBase()}/admin/ad/promote`, {
                method: 'POST',
                headers: await authHeaders({ 'Content-Type': 'application/json' }),
                body: JSON.stringify({ adObjectIds: ids })
            });
            const j = await r.json().catch(() => ({}));
            if (r.ok) { toast.ok(`Promoted/Updated: ${j.affected || ids.length}`); }
            else { toast.err(j.message || 'Promote failed'); }
        } catch (e) {
            console.error(e);
            toast.err('خطأ أثناء الترويج');
        }
    });

    // --- Pagination with bounds
    ui.prev?.addEventListener('click', () => {
        if (state.page > 1) { state.page--; load(); }
    });
    ui.next?.addEventListener('click', () => {
        const maxPage = Math.max(1, Math.ceil((state.total || 0) / state.take));
        if (state.page < maxPage) { state.page++; load(); }
    });

    // --- Flexible action delegation (devices / dhcp)
    function getEntityId(el) {
        if (!el) return '';
        for (const a of ['data-id', 'data-ad-id', 'data-user-id', 'data-device-id']) {
            const v = el.getAttribute?.(a); if (v) return String(v);
        }
        const tr = el.closest?.('tr[data-id]');
        if (tr) return String(tr.getAttribute('data-id') || '');
        return '';
    }

    const endpoints = {
        devices: (id) => `${apiBase()}/admin/devices?adId=${encodeURIComponent(id)}`,
        dhcp: (id) => `${apiBase()}/admin/dhcp/leases?adId=${encodeURIComponent(id)}`
    };

    async function openList(kind, id) {
        if (!id) { toast.err('المعرف غير متاح'); return; }
        try {
            const url = endpoints[kind](id);
            window.open(url, '_blank');
        } catch (e) {
            console.error(e);
            toast.err(`تعذر فتح ${kind.toUpperCase()}`);
        }
    }

    ui.root.addEventListener('click', (ev) => {
        const target = ev.target;

        const devBtn = target.closest?.('[data-action="devices"], .btn-devices, [data-hg="devices"]');
        if (devBtn) {
            ev.preventDefault();
            const id = getEntityId(devBtn);
            openList('devices', id);
            return;
        }

        const dhcpBtn = target.closest?.('[data-action="dhcp"], .btn-dhcp, [data-hg="dhcp"]');
        if (dhcpBtn) {
            ev.preventDefault();
            const id = getEntityId(dhcpBtn);
            openDhcpModal(id);
        }
    });

    // --- Initial load
    load().catch(console.error);
})();