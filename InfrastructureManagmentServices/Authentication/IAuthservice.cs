//using BillingWebFramework.Models;
using InfrastructureManagmentWebFramework.DTOs.Login;
using InfrastructureManagmentWebFramework.Models;
using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Authentication
{
    public interface IAuthservice
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken ct = default);
        Task LogoutAsync();

    }
}
