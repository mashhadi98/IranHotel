using VisitorApp.Contract.Common;
using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class PostEndpointWithoutRequest<TResponse> : EndpointWithoutRequestBase<TResponse>
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Post;

}

public abstract class PostEndpointWithoutRequest<TRequest, TCommand, TResponse> : EndpointWithoutRequestBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{
    protected PostEndpointWithoutRequest(ISender sender) : base(sender)
    {
    }

    public override ApiTypes Type { get; } = ApiTypes.Post;

}

public abstract class PostEndpoint<TRequest, TResponse> : EndpointBase<TRequest, TResponse>
    where TRequest : RequestBase, new()
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Post;
}

public abstract class PostEndpoint<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{

    protected PostEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Post;
}