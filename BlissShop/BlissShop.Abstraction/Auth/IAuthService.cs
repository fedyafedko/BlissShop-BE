using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Responses;
using LanguageExt;

namespace BlissShop.Abstraction.Auth;

public interface IAuthService
{
    Task<RegisterResponse> SignUpAsync(SignUpDTO dto);
    Task<AuthSuccessDTO> SignInAsync(SignInDTO dto);
}
