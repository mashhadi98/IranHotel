using VisitorApp.Application.Common.Services;
using System.Security.Claims;

namespace VisitorApp.API.Common.Services;


public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _http.HttpContext?.User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? UserName =>
        _http.HttpContext?.User?.Identity?.Name
        ?? _http.HttpContext?.User?.FindFirstValue("preferred_username")
        ?? _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}
