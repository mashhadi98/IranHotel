namespace VisitorApp.Contract.Features.Identity.Roles.Delete;

public class DeleteRoleCommandResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "نقش با موفقیت حذف شد";
}
