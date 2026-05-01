using InfrastructureManagmentDataAccess.Repositories;
using InfrastructureManagmentDataAccess;
using InfrastructureManagmentServices.FileStorage;
using InfrastructureManagmentWebFramework.DTOs.Login;
using InfrastructureManagmentWebFramework.DTOs.Profile;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skote.Helpers;

namespace InfrastructureManagmentServices.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IPersonalInformationRepository _piRepo;
        private readonly IRealtimeClient _realtime;
        private readonly IUnitofWork _uow;
        private readonly IFileStorage _files;
        private readonly IHostEnvironment _env; // لاستخدام ContentRoot عند حذف القديم

        public ProfileService(
            IPersonalInformationRepository piRepo,
            IUnitofWork uow,
            IFileStorage files,
            IHostEnvironment env,
            IRealtimeClient realtimeClient)
        {
            _piRepo = piRepo;
            _uow = uow;
            _files = files;
            _env = env;
            _realtime = realtimeClient;
        }

        public async Task<ProfileVm> GetAsync(string userId, CancellationToken ct = default)
        {
            var p = await _piRepo.GetByUserIdAsync(userId, ct); // لو مش موجودة، اعملها في الريبو
            if (p is null) return null;

            return new ProfileVm
            {
                FullName = p.FullName,
                Phone = p.Phone,
                Department = p.Department,
                JobTitle = p.JobTitle,
                Address = p.Address,
                Gender = p.Gender,
                BirthDate = p.BirthDate,
                NationalId = p.NationalId,
                ProfileImagePath = p.ProfileImagePath
            };
        }

        public async Task<AuthResult> UpdateAsync(string userId, ProfileEditDto dto, CancellationToken ct = default)
        {
            var p = await _piRepo.GetByUserIdAsync(userId, ct);
            if (p is null) return new(false, "لم يتم العثور على ملفك الشخصي.");

            // تبديل الصورة إن وُجدت صورة جديدة
            if (dto.NewProfileImage is { Length: > 0 })
            {
                // احفظ الجديدة
                var newPath = await _files.SaveProfileImageAsync(dto.NewProfileImage, userId, ct);

                // احذف القديمة لو تحت wwwroot/uploads/profiles
                if (!string.IsNullOrWhiteSpace(p.ProfileImagePath))
                {
                    var webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
                    var oldAbs = Path.Combine(webRoot, p.ProfileImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    var allowedRoot = Path.Combine(webRoot, "uploads", "profiles");
                    if (oldAbs.StartsWith(allowedRoot) && File.Exists(oldAbs))
                        File.Delete(oldAbs);
                }
                p.ProfileImagePath = newPath;
            }

            // حدّث باقي البيانات
            p.FullName = dto.FullName;
            p.Phone = dto.Phone;
            p.Department = dto.Department;
            p.JobTitle = dto.JobTitle;
            p.Address = dto.Address;
            p.Gender = dto.Gender;
            p.BirthDate = dto.BirthDate;
            p.NationalId = dto.NationalId;

            _uow.Complete();

            // إشعار فوري للمستخدم نفسه
            try
            {
                await _realtime.ToUserAsync(userId, "profile.updated",
                    $"{dto.FullName} updated profile",
                    new { newPhoto = p.ProfileImagePath, name = p.FullName }, ct);
            }
            catch { /* تجاهل أي فشل في الإشعار */ }

            return new(true, "تم تحديث الملف الشخصي.");
        }
    }
}
