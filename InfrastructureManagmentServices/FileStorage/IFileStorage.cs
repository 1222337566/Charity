using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.FileStorage
{
    public interface IFileStorage
    {
        Task<string> SaveProfileImageAsync(IFormFile file, string userId, CancellationToken ct = default);
    }
}
