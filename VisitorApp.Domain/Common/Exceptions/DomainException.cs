namespace VisitorApp.Domain.Common.Exceptions;

/// <summary>
/// Base exception class for domain-specific business logic errors
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 