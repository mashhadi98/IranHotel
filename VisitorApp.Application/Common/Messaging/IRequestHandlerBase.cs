namespace VisitorApp.Application.Abstractions.Messaging;

public interface IRequestHandlerBase<TRequest> : IRequestHandler<TRequest, object>
 where TRequest : IRequestBase
{
}

public interface IRequestHandlerBase<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequestBase<TResponse>
{
}
