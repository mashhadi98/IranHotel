using MediatR;
using VisitorApp.Contract.Features.Identity.RefreshToken;

namespace VisitorApp.Application.Features.Identity.RefreshToken;

public class RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
{
    public required string RefreshToken { get; set; }
    public required string AccessToken { get; set; }
} 