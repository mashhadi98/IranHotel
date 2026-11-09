using VisitorApp.Application.Features.Identity.RefreshToken;
using VisitorApp.Contract.Features.Identity.RefreshToken;

namespace VisitorApp.API.Features.Identity.RefreshToken;

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, RefreshTokenCommandResponse>
{
    public override void Configure()
    {
        Post("/identity/refresh-token");
        AllowAnonymous();
        Tags("Identity");
        Summary(s =>
        {
            s.Summary = "Refresh access token using refresh token";
            s.Description = "Generates new access and refresh tokens using a valid refresh token";
            s.Responses[200] = "Tokens refreshed successfully";
            s.Responses[401] = "Invalid or expired refresh token";
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var command = new RefreshTokenCommandRequest
        {
            RefreshToken = request.RefreshToken,
            AccessToken = request.AccessToken
        };

        var mediator = Resolve<IMediator>();
        var result = await mediator.Send(command, ct);

        await Send.OkAsync(result, ct);
    }
}