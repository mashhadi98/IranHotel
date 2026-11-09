using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using VisitorApp.Domain.Features.Identity.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using VisitorApp.Application.Features.Identity.Common;
using VisitorApp.Contract.Features.Identity.Login;

namespace VisitorApp.Application.Features.Identity.Login;

public class LoginCommandHandler : RequestHandlerBase<LoginCommandRequest, LoginCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    // JWT settings - in production, these should come from configuration
    private const string SecretKey = "VisitorApp-Super-Secret-Key-For-JWT-Token-Generation-2024-MinLength32Chars";
    private const string Issuer = "VisitorApp";
    private const string Audience = "VisitorApp-API";

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOpenIddictApplicationManager applicationManager,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public override async Task<LoginCommandResponse> Handler(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is deactivated");
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("Account is locked due to too many failed attempts");
            }

            throw new UnauthorizedAccessException("Invalid email or password");
        }

                    // Generate real JWT tokens using OpenIddict
            var tokens = await GenerateTokensAsync(user);
            
            return new LoginCommandResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.ExpiresAt,
                IsSuccess = true,
                Message = "Login successful"
            };
    }

    private async Task<TokenResult> GenerateTokensAsync(ApplicationUser user)
    {
        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);
        
        // Create access token
        var accessToken = GenerateJwtToken(user, roles);
        
        // Create refresh token and save to database
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshTokenAsync(user.Id, refreshToken);
        
        // Set expiration time (1 hour for access token)
        var expiresAt = DateTime.UtcNow.AddHours(1);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };
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

    private async Task SaveRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var refreshTokenEntity = new UserRefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        refreshTokenEntity.SetCreated();

        await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);
    }

    private class TokenResult
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}