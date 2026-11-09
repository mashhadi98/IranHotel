using VisitorApp.Application.Features.Identity.Login;
using VisitorApp.Contract.Features.Identity.Login;

namespace VisitorApp.API.Features.Identity.Login;

public class LoginHandler : PostEndpoint<LoginRequest, LoginCommandRequest, LoginCommandResponse>
{
    public override string? RolesAccess => "";

    public override void Configure()
    {
        Summary(s =>
        {
            s.ExampleRequest = new LoginRequest
            {
                Email = "admin@visitorapp.com",
                Password = "Admin123!@#",
                RememberMe = true
            };
        });
        base.Configure();
    }

    public LoginHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
}