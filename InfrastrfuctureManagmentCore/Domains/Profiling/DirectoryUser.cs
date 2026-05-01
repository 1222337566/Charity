namespace InfrastrfuctureManagmentCore.Domains.Progiling;

public class DirectoryUser
{
    public int Id { get; set; }
    public string AdObjectId { get; set; } = default!; // objectGUID كـ GUID نصي
    public string Upn { get; set; } = default!;        // userPrincipalName أو sAMAccountName@domain
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Department { get; set; }
    public string DistinguishedName { get; set; } = default!;
    public DateTime FetchedAtUtc { get; set; } = DateTime.UtcNow;

    // جديد:
    public string? UserWorkstations { get; set; }
}
