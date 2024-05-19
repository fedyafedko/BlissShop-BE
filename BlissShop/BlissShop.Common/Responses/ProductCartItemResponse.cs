using BlissShop.Common.DTO.Products;

namespace BlissShop.Common.Responses;

public class ProductCartItemResponse
{
    public ProductDTO Product { get; set; } = null!;
    public int Quantity { get; set; }
}