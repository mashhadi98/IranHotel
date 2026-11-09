namespace VisitorApp.Domain.Common.Exceptions;

/// <summary>
/// Exception thrown when business logic rules are violated
/// </summary>
public class BusinessLogicException : DomainException
{
    public BusinessLogicException(string message) : base(message)
    {
    }

    public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 