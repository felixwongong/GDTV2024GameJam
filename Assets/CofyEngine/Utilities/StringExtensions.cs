public static class StringExtensions
{
    public static bool isNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool notNullOrEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }
}