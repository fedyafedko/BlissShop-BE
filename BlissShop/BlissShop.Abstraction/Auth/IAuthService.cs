using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;
using LanguageExt;

namespace BlissShop.Abstraction.Auth;

public interface IAuthService
{
    Task<RegisterResponseDTO> SignUpAsync(SignUpDTO dto);
    Task<AuthSuccessDTO> SignInAsync(SignInDTO dto);
}
