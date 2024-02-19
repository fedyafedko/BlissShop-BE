namespace BlissShop.Common.Exceptions;

public class ExpiredException : Exception
{
    public ExpiredException(string? message)
        : base(message) { }
}
