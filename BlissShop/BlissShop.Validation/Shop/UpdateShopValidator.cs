using BlissShop.Common.DTO.Shop;
using FluentValidation;

namespace BlissShop.Validation.Shop;

public class UpdateShopValidator : AbstractValidator<UpdateShopDTO>
{
    public UpdateShopValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Name is required and should not exceed 30 characters");
    }
}