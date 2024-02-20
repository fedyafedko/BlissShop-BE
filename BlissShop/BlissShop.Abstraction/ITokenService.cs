using BlissShop.Common.DTO.Auth;
using BlissShop.Entities;

namespace BlissShop.Abstraction;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<AuthSuccessDTO> GetAuthTokensAsync(User user);
}
