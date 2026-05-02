using InfrastructureManagmentWebFramework.DTOs.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Register
{
    public interface IRegisterationService
    {
        Task<RegisterResult> RegisterWithPersonalInfoAsync(RegisterWithInfoDto dto, CancellationToken ct = default);
    }

}
