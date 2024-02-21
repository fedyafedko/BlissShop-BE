using BlissShop.Common.DTO.Auth;
using BlissShop.Entities;
using LanguageExt;

namespace BlissShop.Abstraction.Auth;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
    Task<AuthSuccessDTO> GetAuthTokensAsync(User user);
}
