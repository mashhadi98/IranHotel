namespace VisitorApp.Domain.Common.DTOs;

public sealed class CursorPaginatedResponse<T>
{
    public CursorPaginatedResponse(ICollection<T> list, bool hasMore, Cursor? nextCursor)
    {
        List = list;
        HasMore = hasMore;
        NextCursor = nextCursor;
    }

    public ICollection<T> List { get; }
    public bool HasMore { get; }
    public Cursor? NextCursor { get; }
}
