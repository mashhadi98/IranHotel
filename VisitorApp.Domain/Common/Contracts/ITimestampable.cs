namespace VisitorApp.Domain.Common.Contracts;

/// <summary>
/// Interface for entities that support optimistic concurrency control using timestamp/rowversion
/// </summary>
public interface ITimestampable
{
    /// <summary>
    /// Timestamp/RowVersion for optimistic concurrency control
    /// This field is automatically managed by the database
    /// </summary>
    byte[] Timestamp { get; set; }
} 