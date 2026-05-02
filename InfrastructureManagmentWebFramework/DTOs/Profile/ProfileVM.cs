// InfrastructureManagmentWebFramework/DTOs/Profile/ProfileDtos.cs
using Microsoft.AspNetCore.Http;
using System;

public class ProfileVm
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public string Address { get; set; }
    public string Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string NationalId { get; set; }
    public string? ProfileImagePath { get; set; }
}

