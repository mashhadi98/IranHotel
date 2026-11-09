using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Users.GetPaginated;

public class GetPaginatedUserRequest : PaginatedRequestBase<GetPaginatedUserFilter>
{
    public GetPaginatedUserRequest() : base("Users", ApiTypes.Get) { }
}

public class GetPaginatedUserFilter
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "FirstName";
    public bool SortDescending { get; set; } = false;
}
