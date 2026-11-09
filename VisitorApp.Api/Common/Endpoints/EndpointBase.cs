using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class EndpointWithoutRequestBase<TResponse> : EndpointWithoutRequest<TResponse>
    where TResponse : class
{
    public abstract ApiTypes Type { get; }
    public abstract string Route { get; }
    public virtual new string? Summary { get; } = null;
    public virtual new string? Description { get; } = null;
    public virtual string? RolesAccess { get; } = "Admin";
    public override void Configure()
    {
        if (Type == ApiTypes.Get)
            Get(Route);
        else if (Type == ApiTypes.Post)
            Post(Route);
        else if (Type == ApiTypes.Put)
            Put(Route);
        else if (Type == ApiTypes.Delete)
            Delete(Route);
        else if (Type == ApiTypes.Patch)
            Patch(Route);
        else
            throw new NotSupportedException($"API type {Type} is not supported.");

        if (RolesAccess is null)
        {
            Options(options => options.RequireAuthorization());
        }
        else if (RolesAccess.Length > 0)
        {
            Roles("Admin");
        }
        else
        {
            AllowAnonymous();
        }

        Summary(s =>
        {
            s.Summary = Summary ?? string.Empty;
            s.Description = Description ?? string.Empty;
            s.Response<TResponse>(200, Summary ?? string.Empty);
        });
    }
    public abstract Task<TResponse> HandlerAsync(CancellationToken cancellationToken);

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var response = await HandlerAsync(cancellationToken);
        await Send.OkAsync((response), cancellationToken);
    }
}

public abstract class EndpointWithoutRequestBase<TRequest, TCommand, TResponse> : EndpointWithoutRequestBase<TResponse>
    where TResponse : class
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<TResponse>, new()
{
    private readonly ISender _sender;
    public TRequest request = new TRequest();
    public override string Route => request.Route;
    protected EndpointWithoutRequestBase(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<TResponse> HandlerAsync(CancellationToken cancellationToken)
    {
        var command = new TCommand();

        var result = await _sender.Send(command, cancellationToken);

        return result;
    }
}

public abstract class EndpointBase<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : RequestBase, new()
    where TResponse : class
{
    public abstract ApiTypes Type { get; }
    public virtual new string? Summary { get; } = null;
    public virtual new string? Description { get; } = null;
    public virtual string? RolesAccess { get; } = "Admin";
    public override void Configure()
    {
        var request = new TRequest();

        if (Type == ApiTypes.Get)
            Get(request.Route.ToLower());
        else if (Type == ApiTypes.Post)
            Post(request.Route.ToLower());
        else if (Type == ApiTypes.Put)
            Put(request.Route.ToLower());
        else if (Type == ApiTypes.Delete)
            Delete(request.Route.ToLower());
        else if (Type == ApiTypes.Patch)
            Patch(request.Route.ToLower());
        else
            throw new NotSupportedException($"API type {Type} is not supported.");

        if (RolesAccess is null)
        {
            Options(options => options.RequireAuthorization());
        }
        else if (RolesAccess.Length > 0)
        {
            Roles("Admin");
        }
        else
        {
            AllowAnonymous();
        }

        Summary(s =>
            {
                s.Summary = Summary ?? string.Empty;
                s.Description = Description ?? string.Empty;
                s.Response<TResponse>(200, Summary ?? string.Empty);
            });
    }
    public abstract Task<TResponse> HandlerAsync(TRequest request, CancellationToken cancellationToken);

    public override async Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var response = await HandlerAsync(request, cancellationToken);
        //await Send.OkAsync(new SuccessResponse<TResponse>(response), cancellationToken);
        await Send.OkAsync(response, cancellationToken);
    }
}

public abstract class EndpointBase<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TResponse>
    where TRequest : RequestBase, new()
    where TResponse : class
    where TCommand : IRequestBase<TResponse>
{
    private readonly ISender _sender;
    private readonly AutoMapper.IMapper _mapper;

    protected EndpointBase(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    public async override Task<TResponse> HandlerAsync(TRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<TCommand>(request);

        var result = await _sender.Send(command, cancellationToken);
        //if (result.IsFailure)
        //    ThrowError(result.Error, 400);

        return result;
    }
}