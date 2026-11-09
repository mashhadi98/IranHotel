using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Features.Identity.Common;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Infrastructure.Features.Identity;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserRefreshToken?> GetActiveRefreshTokenAsync(string token)
    {
        return await _context.UserRefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
    }

    public async Task SaveRefreshTokenAsync(UserRefreshToken refreshToken)
    {
        _context.UserRefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(Guid refreshTokenId, string reason, string? replacedByToken = null)
    {
        var refreshToken = await _context.UserRefreshTokens.FindAsync(refreshTokenId);
        if (refreshToken != null)
        {
            refreshToken.Revoke(reason, replacedByToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
} 