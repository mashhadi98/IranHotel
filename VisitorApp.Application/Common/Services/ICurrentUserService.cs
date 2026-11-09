namespace VisitorApp.Application.Common.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
}