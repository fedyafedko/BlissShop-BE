using BlissShop.Common.DTO.User;
using BlissShop.Validation.Utility;
using FluentValidation;

namespace BlissShop.Validation.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty()
            .Matches(ValidationRegexes.FullNameRegex)
            .WithMessage("Your full name is invalid");
    }
}