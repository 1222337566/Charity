using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Register
{
    public record RegisterResult(bool Succeeded, string Message, string UserId = null, string ImagePath = null);

}
