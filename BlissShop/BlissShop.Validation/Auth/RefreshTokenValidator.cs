using BlissShop.Common.DTO.Auth;
using FluentValidation;

namespace BlissShop.Validation.Auth;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenDTO>
{
    public RefreshTokenValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .WithMessage("Your refresh token is invalid");

        RuleFor(request => request.AccessToken)
            .NotEmpty()
            .WithMessage("Your access token is invalid");
    }
}