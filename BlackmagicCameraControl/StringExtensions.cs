namespace BlackmagicCameraControl;

internal static class StringExtensions
{
    public static string JoinString(this IEnumerable<string> enumerable, string separator = "")
    {
        return string.Join(separator, enumerable);
    }
}