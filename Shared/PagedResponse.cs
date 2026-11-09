namespace Shared
{
    public record PagedResponse<T> (T List, int Count);
}
