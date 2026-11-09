using VisitorApp.Application.Common.Messaging;

namespace VisitorApp.Application.Abstractions.Messaging;

public abstract class RequestHandlerBase<TRequest> : IRequestHandler<TRequest, object>
 where TRequest : IRequestBase
{
    private readonly IEnumerable<IValidatorService<TRequest>> _validators;
    protected RequestHandlerBase()
    {

    }
    protected RequestHandlerBase(IEnumerable<IValidatorService<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<object> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators != null && _validators.Any())
        {
            foreach (var validator in _validators)
            {
                validator.Execute(request);
            }
        }

        await Handler(request, cancellationToken);

        return "success";
    }

    public abstract Task Handler(TRequest request, CancellationToken cancellationToken);
}

public abstract class RequestHandlerBase<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequestBase<TResponse>
{
    private readonly IEnumerable<IValidatorService<TRequest, TResponse>> _validators;
    protected RequestHandlerBase()
    {

    }
    protected RequestHandlerBase(IEnumerable<IValidatorService<TRequest, TResponse>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators != null && _validators.Any())
        {
            foreach (var validator in _validators)
            {
                validator.Execute(request);
            }
        }

        var result = await Handler(request, cancellationToken);

        return result;
    }

    public abstract Task<TResponse> Handler(TRequest request, CancellationToken cancellationToken);

}
