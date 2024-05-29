using BlissShop.Common.Requests;
using FluentValidation;

namespace BlissShop.Validation;

public class SupportRequestValidator : AbstractValidator<SupportRequest>
{
    public SupportRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Your email is invalid");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Your message is empty");
    }
}