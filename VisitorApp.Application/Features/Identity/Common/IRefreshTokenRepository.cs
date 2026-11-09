using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Application.Features.Identity.Common;

public interface IRefreshTokenRepository
{
    Task<UserRefreshToken?> GetActiveRefreshTokenAsync(string token);
    Task SaveRefreshTokenAsync(UserRefreshToken refreshToken);
    Task RevokeRefreshTokenAsync(Guid refreshTokenId, string reason, string? replacedByToken = null);
    Task<int> SaveChangesAsync();
} 