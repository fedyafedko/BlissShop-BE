namespace BlissShop.Common.Exceptions;

public class ConfirmedEmailException : Exception
{
    public ConfirmedEmailException(string? message)
        : base(message) { }
}
