using VisitorApp.Application.Abstractions.Messaging;

namespace VisitorApp.API.Common.Endpoints;

public abstract class DropdownEndpointWithoutRequest<TRequest, TCommand> : EndpointWithoutRequestBase<TRequest, TCommand, List<DropDownDto>>
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<List<DropDownDto>>, new()
{
    protected DropdownEndpointWithoutRequest(ISender sender) : base(sender)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}
public abstract class DropdownEndpointWithoutRequest<TRequest, TCommand, TKey> : EndpointWithoutRequestBase<TRequest, TCommand, List<DropDownDto<TKey>>>
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<List<DropDownDto<TKey>>>, new()
{
    protected DropdownEndpointWithoutRequest(ISender sender) : base(sender)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}

public abstract class DropdownEndpoint<TRequest, TCommand> : EndpointBase<TRequest, TCommand, List<DropDownDto>>
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<List<DropDownDto>>, new()
{
    protected DropdownEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}

public abstract class DropdownEndpoint<TRequest, TCommand, TKey> : EndpointBase<TRequest, TCommand, List<DropDownDto<TKey>>>
    where TRequest : RequestBase, new()
    where TCommand : IRequestBase<List<DropDownDto<TKey>>>, new()
{
    protected DropdownEndpoint(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
    public override ApiTypes Type { get; } = ApiTypes.Get;
}