namespace VisitorApp.Contract.Features.Identity.Users.Delete;

public class DeleteUserCommandResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "کاربر با موفقیت حذف شد";
}
