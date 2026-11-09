namespace VisitorApp.Domain.Common.DTOs;

public class DropDownDto : DropDownDto<Guid>
{
}

public class DropDownDto<TKey>
{
    public required TKey Id { get; set; }
    public required string Title { get; set; }
}
