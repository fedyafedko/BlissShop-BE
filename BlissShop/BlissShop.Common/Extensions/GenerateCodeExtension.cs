namespace BlissShop.Common.Extensions;

public static class GenerateCodeExtension
{

    private static readonly Random random = new Random();

    public static int GenerateCode(this int length)
    {
        int min = (int)Math.Pow(10, length - 1);
        int max = (int)Math.Pow(10, length) - 1;

        return random.Next(min, max);
    }
}
