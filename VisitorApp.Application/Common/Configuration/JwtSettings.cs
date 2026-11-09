namespace VisitorApp.Application.Common.Configuration;

public sealed class JwtSettings
{
    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
    public bool? RequireHttpsMetadata { get; set; }
}
