namespace VisitorApp.Domain.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityType, object id) 
        : base($"Entity of type '{entityType}' with ID '{id}' was not found.")
    {
        EntityType = entityType;
        EntityId = id;
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }

    public string? EntityType { get; }
    public object? EntityId { get; }
} 