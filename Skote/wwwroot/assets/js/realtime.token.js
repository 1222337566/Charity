// wwwroot/js/realtime.token.js (v2)
// Central TokenProvider that fetches JWT from /auth/realtime with auto-refresh, error handling, and configurability.
//
// Usage in _Layout.cshtml:
//   <script>
//     window.__REALTIME_TOKEN_ENDPOINT = "/auth/realtime"; // optional override
//     window.__REALTIME_TOKEN_SKEW = 60; // seconds before expiry to refresh (optional)
//   </script>
//   <script src="~/lib/@microsoft/signalr/dist/browser/signalr.min.js"></script>
//   <script src="~/js/realtime.token.js"></script>
//   <script src="~/js/signalr.manager.js"></script>
//
(function (win) {
    "use strict";

    const DEFAULT_ENDPOINT = "/auth/realtime-token";

   

    const endpoint = win.__REALTIME_TOKEN_ENDPOINT || DEFAULT_ENDPOINT;

    let SKEW = Number(win.__REALTIME_TOKEN_SKEW || 60); // seconds before expiry to refresh

    // Internal state
    let _token = null;
    let _exp = 0;          // unix seconds
    let _inflight = null;  // Promise for a fetch in progress

    // Utilities
    function padB64(s) { return s + "===".slice((s.length + 3) % 4); }
    function b64decode(str) {
        try { return atob(padB64(str).replace(/-/g, '+').replace(/_/g, '/')); }
        catch { return ""; }
    }
    function decodePayload(jwt) {
        try {
            const parts = String(jwt).split(".");
            if (parts.length < 2) return {};
            const json = b64decode(parts[1]);
            return JSON.parse(json || "{}");
        } catch { return {}; }
    }
    function nowSec() { return Math.floor(Date.now() / 1000); }

    async function fetchToken() {
        const url = endpoint;
        const res = await fetch(url, {
            method: "GET",
            credentials: "include",
            headers: { "Accept": "application/json" }
        });

        if (!res.ok) {
            try { win.dispatchEvent(new CustomEvent("auth:required", { detail: { status: res.status } })); } catch { }
            throw new Error("Token endpoint error: HTTP " + res.status);
        }

        let data;
        try {
            data = await res.json();
        } catch {
            throw new Error("Token endpoint did not return JSON");
        }

        const token = data.token || data.access_token || data.jwt || "";
        if (!token) throw new Error("Token endpoint returned no token field");

        const payload = decodePayload(token);
        let exp = 0;

        if (typeof data.expiresAt === "number" && data.expiresAt > 0) {
            exp = data.expiresAt;
        } else if (typeof payload.exp === "number" && payload.exp > 0) {
            exp = payload.exp;
        } else if (typeof data.ttlSec === "number" && data.ttlSec > 0) {
            exp = nowSec() + Number(data.ttlSec);
        } else {
            exp = nowSec() + 600; // default 10 minutes
        }

        _token = token;
        _exp = exp;
        return _token;
    }

    async function getToken() {
        const n = nowSec();
        if (_token && _exp > n + SKEW) return _token;
        if (_inflight) return _inflight;
        _inflight = fetchToken().finally(() => { _inflight = null; });
        return _inflight;
    }

    async function refresh() { _token = null; _exp = 0; return getToken(); }
    function getTokenSync() { return _token || ""; }
    function setSkew(sec) { SKEW = Math.max(0, Number(sec || 0)); }
    async function getAuthHeader() {
        const t = await getToken();
        return { "Authorization": "Bearer " + t };
    }

    win.addEventListener("auth:invalidate", () => { _token = null; _exp = 0; }, { passive: true });

    win.TokenProvider = {
        getToken,
        refresh,
        getTokenSync,
        setSkew,
        getAuthHeader,
        _debug: () => ({ token: _token, exp: _exp, skew: SKEW, endpoint })
    };
})(window);
