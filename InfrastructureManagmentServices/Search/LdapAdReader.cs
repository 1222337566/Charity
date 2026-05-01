using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public class LdapAdReader : IAdReader
    {
        private readonly IConfiguration _cfg;
        public LdapAdReader(IConfiguration cfg) => _cfg = cfg;

        public async Task<IEnumerable<(string Id, string Name, string Email, string OU)>> FindUsersAsync(string q, int take, CancellationToken ct)
        {
            var sec = _cfg.GetSection("Directory");
            var server = sec["LdapServer"];
            var port = int.TryParse(sec["Port"], out var p) ? p : 389;
            var useSsl = bool.TryParse(sec["UseSsl"], out var ssl) && ssl;
            var bindUser = sec["BindUser"];
            var bindPass = sec["BindPassword"];
            var baseDn = sec["BaseDn"];
            var filter = string.Format(sec["UserFilter"] ?? "(&(objectClass=user)(cn=*{0}*))", EscapeLdap(q));

            var attrs = sec.GetSection("Attributes").Get<string[]>() ?? new[] { "cn", "mail", "distinguishedName" };

            return await Task.Run(() =>
            {
                var list = new List<(string Id, string Name, string Email, string OU)>();
                using var conn = new LdapConnection(new LdapDirectoryIdentifier(server, port, false, false));
                conn.SessionOptions.SecureSocketLayer = useSsl;
                conn.Credential = new NetworkCredential(bindUser, bindPass);
                conn.Bind();

                var req = new SearchRequest(baseDn, filter, SearchScope.Subtree, attrs);
                req.SizeLimit = take;
                var resp = (SearchResponse)conn.SendRequest(req);

                foreach (SearchResultEntry e in resp.Entries)
                {
                    string cn = e.Attributes["cn"]?.GetValues(typeof(string)).Cast<string>().FirstOrDefault() ?? "";
                    string mail = e.Attributes["mail"]?.GetValues(typeof(string)).Cast<string>().FirstOrDefault() ?? "";
                    string dn = e.Attributes["distinguishedName"]?.GetValues(typeof(string)).Cast<string>().FirstOrDefault() ?? "";
                    string id = dn; // بنستخدم DN كمُعرّف
                    string ou = ExtractOuFromDn(dn);
                    list.Add((id, cn, mail, ou));
                    if (list.Count >= take) break;
                }
                return list.AsEnumerable();
            }, ct);
        }

        private static string EscapeLdap(string s) =>
            s.Replace("\\", "\\5c").Replace("*", "\\2a").Replace("(", "\\28").Replace(")", "\\29").Replace("\0", "\\00");

        private static string ExtractOuFromDn(string dn)
        {
            // مثال بسيط لاستخراج OU=... من DN
            // CN=Ahmed Hassan,OU=IT,OU=Users,DC=yourdomain,DC=local => IT/Users
            var parts = dn.Split(',').Where(x => x.StartsWith("OU=", StringComparison.OrdinalIgnoreCase))
                                     .Select(x => x.Substring(3));
            return string.Join("/", parts);
        }
    }
}
