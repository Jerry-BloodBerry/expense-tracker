namespace API.Utils;

public class CaseInsensitiveEnum<T> where T : struct, Enum
{
    private CaseInsensitiveEnum(T value) => Value = value;

    public T Value { get; }

    // ReSharper disable once UnusedMember.Global
    public static bool TryParse(string value, out CaseInsensitiveEnum<T> result)
    {
        var tryParse = Enum.TryParse(value, true, out T e);
        result = new CaseInsensitiveEnum<T>(e);

        return tryParse;
    }
}