using BlissShop.Common.DTO.Products;
using FluentValidation;

namespace BlissShop.Validation.Product;

public class CreateProductValidator : AbstractValidator<CreateProductDTO>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Price)
            .GreaterThan(0);
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);
        RuleFor(x => x.Tags)
            .NotEmpty()
            .ForEach(x => x.MaximumLength(50));
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}