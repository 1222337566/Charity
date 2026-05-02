//using BillingCore.Domains.Connection;
//using BillingDataAccess.EntityFramework;
//using BillingSystem.Authentication;
//using BillingWebApi.ActionFilters;
//using BillingWebFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
//using SMSManagementApplication.Authentication;
//using SMSManagementWebFramework.Models;
using InfrastructureManagmentWebFramework.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InfrastructureManagmentServices.Authentication;

namespace SMSManagmentWebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration;
        //private readonly ILogger<AuthController> logger;
        private readonly IAuthservice _authservice;
        public AuthController(IConfiguration configuration, IAuthservice authservice)
        {
            this.configuration = configuration;
            _authservice = authservice;
        }
        
       

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authservice.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authservice.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authservice.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        
    }
}
