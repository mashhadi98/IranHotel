using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Domain.Features.Identity.Entities;

public class UserRefreshToken : Entity
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? RevokedReason { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    
    // Navigation property
    public ApplicationUser? User { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    public void Revoke(string reason, string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedReason = reason;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        SetUpdated(); // Update the UpdatedAt timestamp
    }
} 