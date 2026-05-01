// InfrastructureManagmentWebFramework/DTOs/Auth/LoginDto.cs
using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
