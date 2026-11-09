namespace VisitorApp.Domain.Common.DTOs;

public class Pagination
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class Pagination<TFilter> : Pagination
{
    public TFilter? Filter { get; set; }
    public SortingBy? Sort { get; set; }
}

public enum SortingBy
{
    CreateDate = 1,
    DescendingCreateDate = 10,
    Letters = 2,
    DescendingLetters = 20,
}
