using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Profile
{
    public class ProfileEditDto : ProfileVm
    {
        public IFormFile? NewProfileImage { get; set; }
    }
}
