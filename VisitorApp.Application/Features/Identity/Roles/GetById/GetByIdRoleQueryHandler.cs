using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Roles.GetById;

namespace VisitorApp.Application.Features.Identity.Roles.GetById;

public class GetByIdRoleQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IEnumerable<IValidatorService<GetByIdRoleQueryRequest, GetByIdRoleQueryResponse>> validators) 
    : RequestHandlerBase<GetByIdRoleQueryRequest, GetByIdRoleQueryResponse>(validators)
{
    public override async Task<GetByIdRoleQueryResponse> Handler(GetByIdRoleQueryRequest request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new ArgumentException(ErrorMessages.Roles.NotFound);
        }

        var response = mapper.Map<GetByIdRoleQueryResponse>(role);
        return response;
    }
} 