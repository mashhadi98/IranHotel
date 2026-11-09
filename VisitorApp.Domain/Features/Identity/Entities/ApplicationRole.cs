using Microsoft.AspNetCore.Identity;
using VisitorApp.Domain.Common.Contracts;

namespace VisitorApp.Domain.Features.Identity.Entities;

public class ApplicationRole : IdentityRole<Guid>, IAuditable, IHasConcurrencyVersion
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long Version { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public ApplicationRole()
    {
        Id = Guid.NewGuid();
    }

    public ApplicationRole(string name, string description) : this()
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        SetCreated();
    }

    public void SetCreated()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    public void UpdateRole(string name, string description)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        SetUpdated();
    }

    public void DeactivateRole()
    {
        IsActive = false;
        SetUpdated();
    }

    public void ActivateRole()
    {
        IsActive = true;
        SetUpdated();
    }
} 