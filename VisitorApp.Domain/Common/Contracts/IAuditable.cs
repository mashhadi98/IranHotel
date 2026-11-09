namespace VisitorApp.Domain.Common.Contracts;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    void SetCreated();
    void SetUpdated();
}