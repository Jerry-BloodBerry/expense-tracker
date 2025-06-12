namespace API.Utils.Response;

public record ListResponse<T>(IEnumerable<T> Data);

public static class ListResponse
{
    public static ListResponse<T> Of<T>(IEnumerable<T> data) => new(data);
    public static ListResponse<T> Single<T>(T data) => new(new[] { data });
}