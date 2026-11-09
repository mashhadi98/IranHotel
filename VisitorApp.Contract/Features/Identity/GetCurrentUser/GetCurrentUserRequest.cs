using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.GetCurrentUser;

public class GetCurrentUserRequest() : RequestBase("Identity/Me", ApiTypes.Get)
{
}
