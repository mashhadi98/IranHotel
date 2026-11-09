namespace VisitorApp.Domain.Common.Contracts;

public interface ISoftDeletable
{
    DateTime? RemovedAt { get; set; }
    void SoftDelete();
    void Restore();
}
