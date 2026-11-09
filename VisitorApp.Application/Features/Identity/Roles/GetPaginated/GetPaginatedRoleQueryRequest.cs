using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Roles.GetPaginated;
using VisitorApp.Domain.Common.DTOs;

namespace VisitorApp.Application.Features.Identity.Roles.GetPaginated;

public class GetPaginatedRoleQueryRequest : Pagination<GetPaginatedRoleFilter>, IRequestBase<PaginatedResponse<GetPaginatedRoleQueryResponse>>
{
}

public class GetPaginatedRoleFilter
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
} 