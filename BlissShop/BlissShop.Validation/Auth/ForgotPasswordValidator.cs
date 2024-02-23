using BlissShop.Common.Requests;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Your email is invalid");
    }
}