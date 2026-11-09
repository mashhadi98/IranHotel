namespace VisitorApp.Contract.Features.Identity.RefreshToken;

public class RefreshTokenCommandResponse
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsSuccess { get; set; }
    public required string Message { get; set; }
}
