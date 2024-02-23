using BlissShop.Common.DTO.Shop;
using FluentValidation;

namespace BlissShop.Validation.Shop;

public class CreateShopValidator : AbstractValidator<UpdateShopDTO>
{
    public CreateShopValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Name is required and should not exceed 30 characters");
    }
}
