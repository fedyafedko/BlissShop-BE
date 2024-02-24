using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Requests.ShopAvatar;
using BlissShop.Common.Responses;

namespace BlissShop.Abstraction.Shop;

public interface IShopService
{
    Task<ShopDTO> AddShopAsync(Guid userId, CreateShopDTO dto);
    Task<ShopDTO> GetShopByIdAsync(Guid id);
    Task<ShopDTO> UpdateShopAsync(Guid shopId, UpdateShopDTO dto);
    Task<bool> DeleteShopAsync(Guid sellerId, Guid id);
    Task<IEnumerable<ShopDTO>> GetShopsForSellerAsync(Guid sellerId);
    Task<AvatarResponse> UploadAvatarAsync(UploadShopAvatarRequest request);
    Task<bool> DeleteAvatarAsync(DeleteShopAvatarRequest request);
}
