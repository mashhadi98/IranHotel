using VisitorApp.Contract.Common;
using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class GetEndpointWithoutRequest<TResponse> : EndpointWithoutRequestBase<TResponse>
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Get;

}

public abstract class GetEndpointWithoutRequest<TRequest, TCommand, TResponse> : EndpointWithoutRequestBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{
    protected GetEndpointWithoutRequest(ISender sender) : base(sender)
    {
    }

    public override ApiTypes Type { get; } = ApiTypes.Get;

}

public abstract class GetEndpoint<TRequest, TResponse> : EndpointBase<TRequest, TResponse>
    where TRequest : RequestBase, new()
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Get;
}

public abstract class GetEndpoint<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{

    protected GetEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}