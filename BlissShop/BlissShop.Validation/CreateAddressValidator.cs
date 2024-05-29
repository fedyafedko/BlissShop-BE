using BlissShop.Common.DTO.Address;
using FluentValidation;

namespace BlissShop.Validation;

public class CreateAddressValidator : AbstractValidator<CreateAddressDTO>
{
    public CreateAddressValidator()
    {
        RuleFor(dto => dto.Country)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Country is required and must be less than 100 characters");
        RuleFor(dto => dto.City)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("City is required and must be less than 100 characters");
        RuleFor(dto => dto.Street)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Street is required and must be less than 100 characters");
        RuleFor(dto => dto.ZipCode)
            .NotEmpty()
            .Matches(@"^\d{5}(?:[-\s]\d{4})?$")
            .WithMessage("Invalid ZipCode");
    }
}
