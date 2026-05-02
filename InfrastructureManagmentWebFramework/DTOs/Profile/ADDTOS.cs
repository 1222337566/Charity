using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Profile
{
    public record AdUserDto(string AdObjectId, string Upn, string? Email, string? DisplayName, string? Department, string DistinguishedName, string? UserWorkstations);

}
