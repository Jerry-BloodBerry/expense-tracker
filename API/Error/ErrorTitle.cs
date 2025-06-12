namespace API.Error;

public static class ErrorTitle
{
    private static readonly Dictionary<string, string> Titles = new();

    public static ErrorOr.Error WithTitle(this ErrorOr.Error error, string title)
    {
        Titles[error.Code] = title;

        return error;
    }

    public static string? GetTitleByErrorCode(string code) =>
        Titles.GetValueOrDefault(code);
}