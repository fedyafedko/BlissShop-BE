using BlissShop.Common.DTO.Products;

namespace BlissShop.Abstraction.Product;

public interface IProductCartService
{
    Task<bool> AddToProductCart(Guid userId, AddProductCartDTO dto);
    Task<bool> RemoveFromProductCart(Guid userId, Guid productId);
}
