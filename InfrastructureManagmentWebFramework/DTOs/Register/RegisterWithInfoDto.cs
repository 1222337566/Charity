using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class RegisterWithInfoDto
{
    // حساب
    [Required, MinLength(3)]
    public string UserName { get; set; }
    [Required, MinLength(6)]
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    // Personal info
    [Required] public string FullName { get; set; }
    public string NationalId { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Phone { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public string Address { get; set; }
    public string Gender { get; set; }
    //public string City { get; set; }

    // صورة
    public IFormFile ProfileImage { get; set; }
}
