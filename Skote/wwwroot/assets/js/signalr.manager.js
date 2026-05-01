// wwwroot/js/signalr.manager.js  (robust hub URL resolution)
(function (win) {
    if (!win.signalR) { console.error("SignalR lib not loaded"); return; }

    // read hubs config from window.__SIGNALR_HUBS__ (object or JSON string)
    let hubsConfig = (function () {
        try {
            const raw = win.__SIGNALR_HUBS__;
            if (!raw) return {};
            return (typeof raw === "string") ? JSON.parse(raw) : raw;
        } catch { return {}; }
    })();

    // Prefer TokenProvider if available; otherwise fall back to window.__jwtToken
    const defaultTokenFactory = () =>
        (win.TokenProvider && win.TokenProvider.getToken)
            ? win.TokenProvider.getToken()
            : (win.__jwtToken || "");

    const _connections = new Map();
    const sleep = (ms) => new Promise(r => setTimeout(r, ms));

    function readMeta(name) {
        const el = document.querySelector(`meta[name="signalr:${name}"]`);
        return el && el.content ? el.content : null;
    }
    function readBodyData(name) {
        return document.body ? document.body.getAttribute(`data-signalr-${name}`) : null;
    }

    function resolveHubUrl(name, opts) {
        // 1) explicit
        if (opts && opts.hubUrl) return opts.hubUrl;

        // 2) global config object/string
        if (hubsConfig && typeof hubsConfig === "object" && hubsConfig[name]) return hubsConfig[name];

        // 3) legacy single chat base
        if (name === "chat" && win.__CHAT_HUB_URL_BASE) return win.__CHAT_HUB_URL_BASE;

        // 4) meta tag in <head>
        const fromMeta = readMeta(name);
        if (fromMeta) return fromMeta;

        // 5) data attribute on <body>
        const fromBody = readBodyData(name);
        if (fromBody) return fromBody;

        // 6) final fallback
        const fallback = `/hubs/${name}`;
        console.warn(`[signalr.manager] No hub URL configured for "${name}". Using fallback: ${fallback}`);
        return fallback;
    }

    async function buildConnection(hubUrl, tokenFactory) {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, { accessTokenFactory: tokenFactory || defaultTokenFactory })
            .withAutomaticReconnect()
            .build();

        conn.onclose(async () => {
            for (let i = 0; i < 3; i++) { await sleep(500 * (i + 1)); try { await conn.start(); return; } catch { } }
            console.warn("[signalr.manager] connection closed and retry attempts exhausted for:", hubUrl);
        });

        await conn.start();
        return conn;
    }

    /**
     * Get or create a client for a hub.
     * opts:
     *   - hubUrl?: string (override)
     *   - tokenFactory?: () => string|Promise<string>
     */
    async function get(name, opts = {}) {
        if (_connections.has(name)) return _connections.get(name);

        const hubUrl = resolveHubUrl(name, opts); // لا نرمي Error، عندنا fallback
        const tokenFactory = opts.tokenFactory || defaultTokenFactory;
        const conn = await buildConnection(hubUrl, tokenFactory);

        const api = {
            name,
            url: hubUrl,
            on: (evt, handler) => { conn.on(evt, handler); return api; },
            off: (evt, handler) => { handler ? conn.off(evt, handler) : conn.off(evt); return api; },
            invoke: (method, ...args) => conn.invoke(method, ...args),
            // helpers لأسماء شائعة في الهاب (عدّلها لو مسميّاتك مختلفة)
            joinGroup: (group) => conn.invoke("JoinRoom", group),
            leaveGroup: (group) => conn.invoke("LeaveRoom", group),
            connection: conn,
            state: () => conn.state
        };

        _connections.set(name, api);
        return api;
    }

    // Runtime configuration APIs
    function configure({ hubs } = {}) {
        if (hubs && typeof hubs === "object") {
            hubsConfig = { ...(hubsConfig || {}), ...hubs };
            console.info("[signalr.manager] hubs configured at runtime:", hubsConfig);
        }
    }
    function set(name, url) {
        hubsConfig = hubsConfig || {};
        hubsConfig[name] = url;
        console.info(`[signalr.manager] hub "${name}" set to: ${url}`);
    }

    async function shutdown() {
        for (const [, api] of _connections) {
            try { await api.connection.stop(); } catch { }
        }
        _connections.clear();
    }

    win.SignalR = { get, shutdown, configure, set };
})(window);
