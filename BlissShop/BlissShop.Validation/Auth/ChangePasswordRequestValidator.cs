using BlissShop.Common.Requests;
using BlissShop.Validation.Utility;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.OldPassword)
            .NotEmpty()
            .Matches(ValidationRegexes.PasswordRegex)
            .WithMessage("Your password must be 8 minimum length and must contain at least one uppercase and lowercase letter, one number and one special symbol");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .Matches(ValidationRegexes.PasswordRegex)
            .WithMessage("Your password must be 8 minimum length and must contain at least one uppercase and lowercase letter, one number and one special symbol");
    }
}