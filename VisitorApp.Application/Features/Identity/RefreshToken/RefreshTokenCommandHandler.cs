using Microsoft.AspNetCore.Identity;
using VisitorApp.Domain.Features.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VisitorApp.Application.Features.Identity.Common;
using MediatR;
using VisitorApp.Contract.Features.Identity.RefreshToken;

namespace VisitorApp.Application.Features.Identity.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    // JWT settings - should match LoginCommandHandler
    private const string SecretKey = "VisitorApp-Super-Secret-Key-For-JWT-Token-Generation-2024-MinLength32Chars";
    private const string Issuer = "VisitorApp";
    private const string Audience = "VisitorApp-API";

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate the refresh token
            var storedRefreshToken = await _refreshTokenRepository.GetActiveRefreshTokenAsync(request.RefreshToken);

            if (storedRefreshToken == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            // 2. Validate the access token (extract user info without validating expiration)
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }

            // 3. Ensure the refresh token belongs to the same user as the access token
            if (storedRefreshToken.UserId != userId)
            {
                throw new UnauthorizedAccessException("Token mismatch");
            }

            // 4. Get the user and their roles
            var user = storedRefreshToken.User!;
            var roles = await _userManager.GetRolesAsync(user);

            // 5. Generate new tokens
            var newAccessToken = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken();

            // 6. Revoke the old refresh token and save the new one
            await _refreshTokenRepository.RevokeRefreshTokenAsync(storedRefreshToken.Id, "Replaced by new token", newRefreshToken);

            var newRefreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshTokenEntity);

            // 7. Return the new tokens
            return new RefreshTokenCommandResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsSuccess = true,
                Message = "Tokens refreshed successfully"
            };
        }
        catch (UnauthorizedAccessException)
        {
            // Re-throw authentication errors to be handled by middleware
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred during token refresh: {ex.Message}", ex);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = false, // Don't validate expiration for refresh
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken || 
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        return principal;
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("user_name", user.UserName!),
            new("user_id", user.Id.ToString()),
            new("is_active", user.IsActive.ToString().ToLower()),
            new("jti", Guid.NewGuid().ToString()) // JWT ID
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        // Generate a random refresh token
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + 
               Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
} 