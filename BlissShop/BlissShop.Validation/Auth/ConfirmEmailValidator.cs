using BlissShop.Common.DTO.Auth;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailDTO>
{
    public ConfirmEmailValidator()
    {
        RuleFor(dto => dto.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
        RuleFor(dto => dto.Code)
            .NotEmpty()
            .WithMessage("Code is required");
    }
}
