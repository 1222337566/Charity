using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Login
{
    public record AuthResult(bool Succeeded, string Message = null);

}
