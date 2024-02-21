using BlissShop.Common.DTO.Auth;

namespace BlissShop.Abstraction.Auth;

public interface IGoogleAuthService
{
    Task<AuthSuccessDTO> GoogleSignUpAsync(string authorizationCode, string Role);
    Task<AuthSuccessDTO> GoogleSignInAsync(string authorizationCode);
}
