namespace VisitorApp.API.Common.Models;

public sealed record SuccessResponse(object? Data = null)
{
    public string Message { get; } = "Success";
    public object? Data { get; set; } = Data;
}
public sealed record SuccessResponse<T>(T Data)
{
    public string Message { get; } = "Success";
    public T Data { get; set; } = Data;
}