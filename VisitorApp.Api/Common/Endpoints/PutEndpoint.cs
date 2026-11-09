using VisitorApp.Contract.Common;
using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class PutEndpointWithoutRequest<TResponse> : EndpointWithoutRequestBase<TResponse>
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Put;

}

public abstract class PutEndpointWithoutRequest<TRequest, TCommand, TResponse> : EndpointWithoutRequestBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{
    protected PutEndpointWithoutRequest(ISender sender) : base(sender)
    {
    }

    public override ApiTypes Type { get; } = ApiTypes.Put;

}

public abstract class PutEndpoint<TRequest, TResponse> : EndpointBase<TRequest, TResponse>
    where TRequest : RequestBase, new()
    where TResponse : class
{
    public override ApiTypes Type { get; } = ApiTypes.Put;
}

public abstract class PutEndpoint<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TCommand, TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{

    protected PutEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Put;
}