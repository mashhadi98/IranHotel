namespace VisitorApp.Contract.Features.Identity.Common;

public class UserInfoDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public List<string> Roles { get; set; } = new();
    public string? PhoneNumber { get; set; }
}
