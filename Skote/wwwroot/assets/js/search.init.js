(function(){
  "use strict";
  const input = document.getElementById('skote-search');
  const dd = document.getElementById('skote-search-dd');
  if(!input || !dd) return;

  const API = (window.appBase || '') + '/api/search';
  const auth = () => (window.getJwtToken ? window.getJwtToken() : null);

  let timer = null;
  function debounceRun(fn, ms){ clearTimeout(timer); timer = setTimeout(fn, ms); }

  function showDD(html){
    dd.innerHTML = html;
    dd.classList.add('show');
    dd.style.display='block';
  }
  function hideDD(){
    dd.classList.remove('show');
    dd.style.display='none';
  }

  function tplSection(title, rows){
    if(!rows || !rows.length) return '';
    const lis = rows.map(r=>`<li class="list-group-item d-flex justify-content-between align-items-center">
      <div>
        <div class="fw-bold">${escapeHtml(r.name || r.ip || '')}</div>
        <div class="small text-muted">${escapeHtml(r.email || r.mac || r.ou || r.scope || '')}</div>
      </div>
    </li>`).join('');
    return `<div class="p-2 border-bottom"><strong>${title}</strong></div>
            <ul class="list-group list-group-flush">${lis}</ul>`;
  }

  function escapeHtml(s){ return String(s ?? '').replace(/[&<>"']/g, m => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[m]); }

  async function doSearch(q){
    if(!q || q.trim().length < 2){ hideDD(); return; }
    const headers = { 'Accept':'application/json' };
    const token = auth();
    if(token) headers['Authorization'] = 'Bearer ' + token;
    const url = `${API}?q=${encodeURIComponent(q)}&take=10`;
    try{
      const r = await fetch(url,{headers});
      if(!r.ok) throw new Error('HTTP '+r.status);
      const j = await r.json();
      const html = [
        tplSection('Users', j.users),
        tplSection('Devices', j.devices),
        tplSection('DHCP', j.dhcp),
        `<div class="p-2 small text-muted text-end">~${(j.meta?.elapsedMs ?? 0)} ms</div>`
      ].join('');
      showDD(html || '<div class="p-2 text-muted small">No results</div>');
    }catch(e){
      console.error(e);
      showDD('<div class="p-2 text-danger small">Error loading results</div>');
    }
  }

  // Debounced typing
  input.addEventListener('input', ()=> debounceRun(()=> doSearch(input.value), 250));

  // Hide on blur
  input.addEventListener('blur', ()=> setTimeout(hideDD, 150));

  // Keyboard shortcut Ctrl+K
  document.addEventListener('keydown', (e)=>{
    if((e.ctrlKey || e.metaKey) && e.key.toLowerCase()==='k'){
      e.preventDefault();
      input.focus();
      input.select();
    }
  });
})();