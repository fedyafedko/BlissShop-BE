using BlissShop.Common.DTO.Auth;
using LanguageExt;

namespace BlissShop.Abstraction;

public interface IEmailConfirmationService
{
    Task<AuthSuccessDTO> ConfirmEmailAsync(ConfirmEmailDTO dto);
    Task<int> GenerateEmailCodeAsync(Guid userId);
    Task ResendConfirmationCodeAsync(Guid userId);
}
