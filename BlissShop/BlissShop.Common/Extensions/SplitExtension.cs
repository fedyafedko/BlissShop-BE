namespace BlissShop.Common.Extensions;

public static class SplitExtension
{
    public static List<string> SplitToList(this string str, string separator)
    {
        return str.Split(separator).ToList();
    }
}
