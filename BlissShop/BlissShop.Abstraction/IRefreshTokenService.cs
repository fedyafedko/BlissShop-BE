using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;

namespace BlissShop.Abstraction;

public interface IRefreshTokenService
{
    Task<AuthSuccessDTO> RefreshTokenAsync(RefreshTokenDTO dto);
}
