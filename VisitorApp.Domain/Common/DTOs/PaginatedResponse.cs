namespace VisitorApp.Domain.Common.DTOs;

public class PaginatedResponse<T>
{
    public PaginatedResponse(ICollection<T> list, int count)
    {
        List = list;
        Count = count;
    }

    public ICollection<T> List { get; set; }
    public int Count { get; set; }
}
