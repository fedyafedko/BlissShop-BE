using BlissShop.Common.DTO.Auth;
using BlissShop.Validation.Utility;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class SignInValidator : AbstractValidator<SignInDTO>
{
    public SignInValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Your email is invalid");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .Matches(ValidationRegexes.PasswordRegex)
            .WithMessage("Your password must be 8 minimum length and must contain at least one uppercase and lowercase letter, one number and one special symbol");
    }
}
