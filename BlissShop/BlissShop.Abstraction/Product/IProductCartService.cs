using BlissShop.Common.DTO.Products;
using BlissShop.Common.Responses;

namespace BlissShop.Abstraction.Product;

public interface IProductCartService
{
    Task<bool> AddToProductCart(Guid userId, AddProductCartDTO dto);
    Task<bool> RemoveFromProductCart(Guid userId, Guid productId);
    Task<ProductCartResponse> GetProductCart(Guid userId);
}
