using InfrastructureManagmentWebFramework.DTOs.Login;
using InfrastructureManagmentWebFramework.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface IProfileService
    {
        public Task<ProfileVm> GetAsync(string userId, CancellationToken ct = default);
        public Task<AuthResult> UpdateAsync(string userId, ProfileEditDto dto, CancellationToken ct = default);


    }
}
