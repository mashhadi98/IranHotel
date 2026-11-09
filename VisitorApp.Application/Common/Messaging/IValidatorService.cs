namespace VisitorApp.Application.Common.Messaging;
public interface IValidatorService<TRequest> where TRequest : IRequestBase
{
    void Execute(TRequest model);
}

public interface IValidatorService<TRequest, TResponse> where TRequest : IRequestBase<TResponse>
{
    void Execute(TRequest request);
}
