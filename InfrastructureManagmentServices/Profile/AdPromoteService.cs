using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    using Microsoft.AspNetCore.Identity;
    using System.Security.Claims;

    public class AdPromoteService : IAdPromoteService
    {
        private readonly IDirectoryUserRepository _repo;
        private readonly UserManager<ApplicationUser> _users; // أو ApplicationUser لو عندك
        public AdPromoteService(IDirectoryUserRepository repo, UserManager<ApplicationUser> users)
        { _repo = repo; _users = users; }

        public async Task<int> PromoteAsync(IEnumerable<string> adObjectIds, CancellationToken ct)
        {
            var list = await _repo.GetByAdIdsAsync(adObjectIds, ct);
            int affected = 0;

            foreach (var du in list)
            {
                ct.ThrowIfCancellationRequested();

                // 1) أنشئ/حدّث المستخدم “بالحقول القياسية فقط”
                var user = (du.Upn != null ? await _users.FindByNameAsync(du.Upn) : null)
                        ?? (du.Email != null ? await _users.FindByEmailAsync(du.Email) : null);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = du.Upn ?? du.Email ?? du.AdObjectId,
                        Email = du.Email ?? du.Upn,
                        EmailConfirmed = true,
                        PhoneNumber = null
                    };
                    var r = await _users.CreateAsync(user);
                    if (!r.Succeeded) continue;
                    affected++;
                }
                else
                {
                    // ممكن تحدث Email لو حابب (اختياري)
                    if (!string.Equals(user.Email, du.Email ?? du.Upn, StringComparison.OrdinalIgnoreCase))
                    {
                        user.Email = du.Email ?? du.Upn;
                        await _users.UpdateAsync(user);
                    }
                    affected++;
                }

                // 2) ضيف/حدّث Claims بدل أعمدة DB
                // هنستخدم أسماء Claims ثابتة
                await UpsertClaimAsync(user, ClaimTypes.Name, du.DisplayName ?? du.Upn ?? du.Email ?? du.AdObjectId);
                await UpsertClaimAsync(user, "ad_object_id", du.AdObjectId);
                await UpsertClaimAsync(user, "department", du.Department ?? "");
                await UpsertClaimAsync(user, "upn", du.Upn ?? "");
                await UpsertClaimAsync(user, "dn", du.DistinguishedName ?? "");
            }

            return affected;
        }

        private async Task UpsertClaimAsync(ApplicationUser user, string type, string value)
        {
            var claims = await _users.GetClaimsAsync(user);
            var exist = claims.FirstOrDefault(c => c.Type == type);
            if (exist == null && !string.IsNullOrEmpty(value))
                await _users.AddClaimAsync(user, new Claim(type, value));
            else if (exist != null && exist.Value != value)
            {
                await _users.ReplaceClaimAsync(user, exist, new Claim(type, value));
            }
        }
    }

}
