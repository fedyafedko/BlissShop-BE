using BlissShop.Common.DTO.Shop;

namespace BlissShop.Abstraction.Shop;

public interface IShopService
{
    Task<ShopDTO> AddShopAsync(Guid userId, CreateShopDTO dto);
    Task<ShopDTO> GetShopByIdAsync(Guid id);
    Task<ShopDTO> UpdateShopAsync(Guid shopId, UpdateShopDTO dto);
    Task<bool> DeleteShopAsync(Guid sellerId, Guid id);
    Task<IEnumerable<ShopDTO>> GetShopsForSellerAsync(Guid sellerId);
}
