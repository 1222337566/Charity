//using BillingCore.Domains.Identity;
//using BillingSystem.Helpers;
//using BillingSystem.Models;
//using BillingWebFramework.Helpers;
//using BillingWebFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using InfrastructureManagmentWebFramework.Helpers;
using InfrastructureManagmentWebFramework.Models;
using InfrastructureManagmentCore.Domains.Identity;
//using InfrastructureManagmentWebFramework.Helpers;
//using InfrastructureManagmentWebFramework.Models;
using System;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InfrastructureManagmentWebFramework.Models;
using InfrastructureManagmentWebFramework.DTOs.Login;
//using Microsoft.AspNet.Identity.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Authentication
{
    public class AuthService : IAuthservice
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserActivityService _activity;
        private readonly RoleManager<IdentityRole> _roleManager;
        private  readonly JWT _jwt;
        public AuthService(SignInManager<ApplicationUser> sm, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IUserActivityService activity)
        {
            _signInManager = sm;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _activity = activity;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                return new AuthModel() { Message = "UserName is Already registered" };
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,



            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel() { Message = errors };

                
            }
            await _userManager.AddToRoleAsync(user, "HTS");


            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {

                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };

        }

        public async Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var conn = _signInManager.Context.Connection.RemoteIpAddress.ToString();
            Console.WriteLine("=== Connection String in Use (AuthService) ===");
            Console.WriteLine(conn);
            Console.WriteLine("==============================================");
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user is null) return new(false, "اسم المستخدم أو كلمة السر غير صحيحة.");

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);
            if (!result.Succeeded) return new(false, "اسم المستخدم أو كلمة السر غير صحيحة.");

            await _activity.LogAsync(user.Id, "Login", ct);
            return new(true);
        }

        public async Task LogoutAsync()
        {
            // مين المستخدم الحالي؟
            var principal = _signInManager.Context?.User;
            var user = principal is null ? null : await _userManager.GetUserAsync(principal);

            await _signInManager.SignOutAsync();

            if (user != null)
            {
                // مش هنفشل الـ Logout لو اللوج فشل، بس نحاول
                try { await _activity.LogAsync(user.Id, "Logout"); } catch { /* ignore */ }
            }
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Username or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }


        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
   
        
    
    }
}
