using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Skote.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Skote.Controllers
{
    [Authorize]
    [ApiController]
    public class RealtimeAuthController : ControllerBase
    {
        private readonly IOptions<JwtOptions> _opt;
        public RealtimeAuthController(IOptions<JwtOptions> opt) => _opt = opt;

        [HttpGet("/auth/realtime-token")]
        public IActionResult Get()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var uname = User.Identity?.Name ?? uid;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, uid), new(ClaimTypes.Name, uname) };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: _opt.Value.Issuer,
                audience: _opt.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
