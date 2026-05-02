(function(){
  const KEY='skote:theme';
  function apply(t){ document.documentElement.setAttribute('data-theme', t); }
  function get(){ return localStorage.getItem(KEY) || 'light'; }
  function set(t){ localStorage.setItem(KEY,t); apply(t); }
  // init
  apply(get());
  // bind toggle button if exists
  document.addEventListener('click',(e)=>{
    const btn = e.target.closest('[data-action="toggle-theme"]');
    if(!btn) return;
    const cur = get();
    set(cur==='dark' ? 'light' : 'dark');
  });
})();