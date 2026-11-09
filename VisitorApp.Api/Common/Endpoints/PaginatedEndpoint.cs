using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class PaginatedEndpoint<TRequest, TResponse> : EndpointBase<TRequest, PaginatedResponse<TResponse>>
    where TRequest : PaginatedRequestBase, new()
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Get;
}

public abstract class PaginatedEndpoint<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TCommand, PaginatedResponse<TResponse>>
    where TRequest : RequestBase, new()
    where TResponse : class
    where TCommand : Pagination, IRequestBase<PaginatedResponse<TResponse>>
{
    protected PaginatedEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}