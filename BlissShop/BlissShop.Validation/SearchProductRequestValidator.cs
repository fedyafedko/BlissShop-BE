using BlissShop.Common.Requests;
using FluentValidation;

namespace BlissShop.Validation;

public class SearchProductRequestValidator : AbstractValidator<SearchProductRequest>
{
    public SearchProductRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");
    }
}
