using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;
using LanguageExt;

namespace BlissShop.Abstraction;

public interface IAuthService
{
    Task<AuthSuccessDTO> SignUpAsync(SignUpDTO dto);
    Task<AuthSuccessDTO> SignInAsync(SignInDTO dto);
}
