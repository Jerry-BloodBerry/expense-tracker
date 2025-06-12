namespace API.Utils.Response;

public record SingleResponse<T>(T Data);

public static class SingleResponse
{
    public static SingleResponse<T> Of<T>(T data) => new(data);
}