using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Users.GetById;

namespace VisitorApp.Application.Features.Identity.Users.GetById;

public class GetByIdUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IEnumerable<IValidatorService<GetByIdUserQueryRequest, GetByIdUserQueryResponse>> validators) 
    : RequestHandlerBase<GetByIdUserQueryRequest, GetByIdUserQueryResponse>(validators)
{
    public override async Task<GetByIdUserQueryResponse> Handler(GetByIdUserQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new ArgumentException(ErrorMessages.Users.NotFound);
        }

        var response = mapper.Map<GetByIdUserQueryResponse>(user);
        return response;
    }
} 