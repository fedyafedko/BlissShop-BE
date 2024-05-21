using BlissShop.Common.DTO.Rating;
using FluentValidation;

namespace BlissShop.Validation.Rating;

public class CreateRatingValidator : AbstractValidator<CreateRatingDTO>
{
    public CreateRatingValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product Id is required");

        RuleFor(x => x.Rate)
            .InclusiveBetween(0, 5)
            .WithMessage("Rate must be between 0 and 5");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Comment must be less than 500 characters");
    }
}
