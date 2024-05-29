using BlissShop.Common.Requests;
using BlissShop.Validation.Utility;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Your email is invalid");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .Matches(ValidationRegexes.PasswordRegex)
            .WithMessage("Your password must be 8 minimum length and must contain at least one uppercase and lowercase letter, one number and one special symbol");

        RuleFor(request => request.ResetToken)
            .NotEmpty();
    }
}