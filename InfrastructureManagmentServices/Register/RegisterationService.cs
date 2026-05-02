using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentDataAccess.Repositories;
using InfrastructureManagmentDataAccess;
using InfrastructureManagmentServices.FileStorage;
using InfrastructureManagmentWebFramework.DTOs.Register;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Register
{
    public class RegisterationService : IRegisterationService
    {
        

    private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _files;
        private readonly IPersonalInformationRepository _piRepo;
        private readonly IUnitofWork _uow; // لحفظ/ترانزاكشن إن لزم

        public RegisterationService(UserManager<ApplicationUser> userManager, IFileStorage files,
            IPersonalInformationRepository piRepo, IUnitofWork uow)
        { _userManager = userManager; _files = files; _piRepo = piRepo; _uow = uow; }

        public async Task<RegisterResult> RegisterWithPersonalInfoAsync(RegisterWithInfoDto dto, CancellationToken ct = default)
        {
            if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                return new(false, "اسم المستخدم موجود بالفعل.");

            var user = new ApplicationUser { UserName = dto.UserName };
            var create = await _userManager.CreateAsync(user, dto.Password);
            if (!create.Succeeded)
                return new(false, string.Join(" | ", create.Errors.Select(e => e.Description)));

            string img = null;
            try { img = await _files.SaveProfileImageAsync(dto.ProfileImage, user.Id, ct); }
            catch (Exception e) { await _userManager.DeleteAsync(user); return new(false, e.Message); }

             _piRepo.Add(new PersonalInformation
            {
                UserId=user.Id,
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                BirthDate = dto.BirthDate,
                Phone = dto.Phone,
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                Address = dto.Address,
                Gender = dto.Gender,
                ProfileImagePath= img
            });

             _uow.Complete();
            return new(true, "تم إنشاء الحساب بنجاح.", user.Id, img);
        }
    }
    }


