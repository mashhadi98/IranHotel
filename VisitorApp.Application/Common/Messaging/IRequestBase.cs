namespace VisitorApp.Application.Abstractions.Messaging;

public interface IRequestBase : IRequest<object>
{
}

public interface IRequestBase<TResponse> : IRequest<TResponse>
{
}
