using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.FileStorage
{
    public class FileSystemFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        public FileSystemFileStorage(IWebHostEnvironment env) => _env = env;

        public async Task<string> SaveProfileImageAsync(IFormFile file, string userId, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new InvalidOperationException("امتداد غير مسموح.");

            const long maxBytes = 2 * 1024 * 1024;
            if (file.Length > maxBytes) throw new InvalidOperationException("الحجم يتجاوز 2MB.");

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadRoot = Path.Combine(webRoot, "uploads", "profiles");
            Directory.CreateDirectory(uploadRoot);

            var fileName = $"{userId}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadRoot, fileName);
            await using (var fs = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(fs, ct);

            return $"/uploads/profiles/{fileName}";
        }
    }
}
