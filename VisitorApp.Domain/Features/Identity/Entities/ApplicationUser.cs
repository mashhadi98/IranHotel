using Microsoft.AspNetCore.Identity;
using VisitorApp.Domain.Common.Contracts;

namespace VisitorApp.Domain.Features.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable, IHasConcurrencyVersion, IEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long Version { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }

    public ApplicationUser(string email, string firstName, string lastName) : this()
    {
        Email = email;
        UserName = email;
        FirstName = firstName;
        LastName = lastName;
        SetCreated();
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void SetCreated()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        SetUpdated();
    }

    public void DeactivateUser()
    {
        IsActive = false;
        SetUpdated();
    }

    public void ActivateUser()
    {
        IsActive = true;
        SetUpdated();
    }

    // Navigation property for refresh tokens
    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}