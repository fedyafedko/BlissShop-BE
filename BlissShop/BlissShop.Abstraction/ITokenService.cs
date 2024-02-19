using BlissShop.Entities;

namespace BlissShop.Abstraction;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
}
