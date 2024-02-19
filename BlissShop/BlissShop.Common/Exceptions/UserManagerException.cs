using Microsoft.AspNetCore.Identity;

namespace BlissShop.Common.Exceptions;

public class UserManagerException : Exception
{
    public UserManagerException(string message, IEnumerable<IdentityError> errors)
        : base($"{message} {string.Join("\n", errors.Select(e => e.Description))}")
    {
        Errors = errors;
    }

    public IEnumerable<IdentityError> Errors { get; }
}
