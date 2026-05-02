using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    using InfrastructureManagmentWebFramework.DTOs.Profile;
    using Microsoft.Extensions.Configuration;
    using System.DirectoryServices.Protocols;
    using System.Net;
    using System.Text;
    using static System.Net.Mime.MediaTypeNames;

    public class LdapDirectoryService : ILdapDirectoryService
    {
        private readonly IConfiguration _cfg;
        public LdapDirectoryService(IConfiguration cfg) => _cfg = cfg;

        private LdapConnection CreateConnection()
        {
            var server = _cfg["LDAP:Server"]!;
            var port = int.TryParse(_cfg["LDAP:Port"], out var p) ? p : 389;
            var useSSL = bool.TryParse(_cfg["LDAP:UseSSL"], out var ssl) && ssl;

            var conn = new LdapConnection(new LdapDirectoryIdentifier(server, port, false, false))
            {
                AuthType = AuthType.Basic
            };

            // LDAP v3
            conn.SessionOptions.ProtocolVersion = 3;
            conn.SessionOptions.SecureSocketLayer = useSSL;
            // اختياري: أوقف الـreferrals لو بتسبب مشاكل
            conn.SessionOptions.ReferralChasing = ReferralChasingOptions.None;

            var bindUser = _cfg["LDAP:BindDn"];     
            // ممكن يكون DN أو UPN أو sAMAccountName
            var bindPass = _cfg["LDAP:BindPassword"];
            var domain = _cfg["LDAP:Domain"];      // اختياري لو هتستخدم sAMAccountName

            if (string.IsNullOrWhiteSpace(bindUser) || string.IsNullOrWhiteSpace(bindPass))
                throw new InvalidOperationException("LDAP Bind credentials are missing.");

            NetworkCredential cred;

            if (bindUser.Contains("@") || bindUser.Contains("CN=") || bindUser.Contains("OU="))
            {
                // UPN أو DN
                cred = new NetworkCredential(bindUser, bindPass);
            }
            else if (!string.IsNullOrWhiteSpace(domain))
            {
                // sAMAccountName + Domain
                cred = new NetworkCredential(bindUser, bindPass, domain);
            }
            else
            {
                // كـ fallback: جرّب UPN من دومين الإعدادات لو موجود
                var upnDomain = _cfg["LDAP:UpnDomain"]; // مثال: yourdomain.local
                cred = !string.IsNullOrWhiteSpace(upnDomain)
                    ? new NetworkCredential($"{bindUser}@{upnDomain}", bindPass)
                    : new NetworkCredential(bindUser, bindPass);
            }

            // **هنا الـBind الفعلي**
            conn.Bind(cred); // لو فشل → هيطلع استثناء واضح (invalid credentials/SSL..)

            return conn; // نفس الـconnection ده هو اللي هنستخدمه في Search
        }

        public async Task<bool> PingAsync(CancellationToken ct)
        {
            using var conn = CreateConnection();
            // لو الـBind نجح، الـPing ناجح
            return await Task.FromResult(true);
        }
        private static string EscapeLdapFilterValue(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var sb = new StringBuilder(value.Length);
            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '\\': sb.Append(@"\5c"); break; // backslash
                    case '*': sb.Append(@"\2a"); break; // asterisk
                    case '(': sb.Append(@"\28"); break;
                    case ')': sb.Append(@"\29"); break;
                    case '\0': sb.Append(@"\00"); break;
                    default: sb.Append(ch); break;
                }
            }
            return sb.ToString();
        }
        public async Task<(IReadOnlyList<AdUserDto> users, string? nextCookie)> SearchUsersAsync(
     string? query, string? ouDn, string? pageCookie, int pageSize, CancellationToken ct)
        {
            using var conn = CreateConnection();
            // 1) تأكيد بروتوكول v3
            conn.SessionOptions.ProtocolVersion = 3;

            var baseDn = string.IsNullOrWhiteSpace(ouDn) ? _cfg["LDAP:BaseDn"]! : ouDn;
            var attrs = _cfg.GetSection("LDAP:Attributes").Get<string[]>()
                        ?? new[] { "distinguishedName", "displayName", "department", "mail", "userPrincipalName", "objectGUID" };

            var rawQ = query?.Trim();
            var safeQ = EscapeLdapFilterValue(rawQ);
            var starQ = string.IsNullOrWhiteSpace(safeQ) ? "*" : $"*{safeQ}*";

            // ملاحظة: لا تهرّب أسماء الخصائص، نهرب القِيَم فقط
            var filter =
                "(&" +
                "(objectClass=user)" +
                "(!(objectClass=computer))" +
                "(|" +
                  $"(displayName={starQ})" +
                  $"(mail={starQ})" +
                  $"(userPrincipalName={starQ})" +
                  $"(sAMAccountName={starQ})" +
                ")" +
                ")";

            var req = new SearchRequest(baseDn, filter, SearchScope.Subtree, attrs);

            // نحاول paging لو السيرفر بيدعم
            var pageCtrl = new PageResultRequestControl(pageSize) { IsCritical = false };
            if (!string.IsNullOrEmpty(pageCookie))
                pageCtrl.Cookie = Convert.FromBase64String(pageCookie); // استخدم Base64 للكوكي
            req.Controls.Add(pageCtrl);

            try
            {
                var resp = (SearchResponse)conn.SendRequest(req);
                return ParseResponse(resp);
            }
            catch (Exception ex) 
            {
                // Fallback: بدون paging + SizeLimit
                req.Controls.Clear();
                req.SizeLimit = pageSize;
                var resp = (SearchResponse)conn.SendRequest(req);
                var (items, _) = ParseResponse(resp);
                return (items, null);
            }

            // Local function لقراءة النتيجة + كوكي الصفحة
            (IReadOnlyList<AdUserDto> items, string? nextCookieBase64) ParseResponse(SearchResponse resp)
            {
                var list = new List<AdUserDto>(resp.Entries.Count);
                foreach (SearchResultEntry e in resp.Entries)
                {
                    ct.ThrowIfCancellationRequested();
                    var dn = e.DistinguishedName;
                    var name = e.Attributes["displayName"]?[0]?.ToString();
                    var dept = e.Attributes["department"]?[0]?.ToString();
                    var mail = e.Attributes["mail"]?[0]?.ToString();
                    var upn = e.Attributes["userPrincipalName"]?[0]?.ToString();
                    var gB = e.Attributes["objectGUID"]?[0] as byte[];
                    string? workstations = null;
                    if (e.Attributes["userWorkstations"] is { Count: > 0 })
                    {
                        // AD بيرجعها كسلسلة مفصولة بفواصل
                        workstations = e.Attributes["userWorkstations"][0]?.ToString();
                    }
                    if (gB == null) continue;
                    var adId = new Guid(gB).ToString();
                    list.Add(new AdUserDto(adId, upn ?? mail ?? adId, mail, name, dept, dn,workstations));
                }

                string? next = null;
                foreach (DirectoryControl c in resp.Controls)
                {
                    if (c is PageResultResponseControl p && p.Cookie is { Length: > 0 })
                    {
                        next = Convert.ToBase64String(p.Cookie); // خزّنه Base64
                        break;
                    }
                }
                return (list, next);
            }

            bool IsControlNotSupported(DirectoryOperationException ex)
            {
                // رسائل شائعة لما السيرفر لا يدعم الـcontrol
                var msg = ex.Message?.ToLowerInvariant() ?? "";
                return msg.Contains("does not support the control");
            }
        }

        public async Task<IReadOnlyList<AdUserDto>> SearchUsersOnceAsync(string? query, string? ouDn, int take, CancellationToken ct)
        {
            var (users, _) = await SearchUsersAsync(query, ouDn, null, take, ct);
            return users;
        }
    }

}
