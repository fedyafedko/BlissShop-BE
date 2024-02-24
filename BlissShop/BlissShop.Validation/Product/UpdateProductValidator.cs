using BlissShop.Common.DTO.Products;
using FluentValidation;

namespace BlissShop.Validation.Product;

public class UpdateProductValidator : AbstractValidator<UpdateProductDTO>
{
    public UpdateProductValidator()
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