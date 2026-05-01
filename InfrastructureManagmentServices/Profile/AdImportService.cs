using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    using InfrastrfuctureManagmentCore.Domains.Progiling;
    using InfrastructureManagmentCore.Domains.Identity;
    using InfrastructureManagmentWebFramework.DTOs.Profile;
    using Microsoft.AspNetCore.Identity;
    using System.Threading;

    public class AdImportService : IAdImportService
    {
        private readonly UserManager<ApplicationUser> _users;
        public AdImportService(UserManager<ApplicationUser> users) => _users = users;

        public Task<int> ImportUsersAsync(IEnumerable<AdUserDto> users, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

}
