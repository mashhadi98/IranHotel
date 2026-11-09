namespace VisitorApp.Domain.Common.Exceptions;

/// <summary>
/// Simple concrete domain exception for testing and basic domain errors
/// </summary>
public class SimpleDomainException : DomainException
{
    public SimpleDomainException(string message) : base(message)
    {
    }

    public SimpleDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 