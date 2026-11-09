namespace VisitorApp.Contract.Features.Identity.RefreshToken;

public class RefreshTokenRequest
{
    public required string RefreshToken { get; set; }
    public required string AccessToken { get; set; }
}
