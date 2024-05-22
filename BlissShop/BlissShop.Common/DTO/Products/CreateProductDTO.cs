using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.DTO.Products;

public class CreateProductDTO
{
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = null!;
    public int Quantity { get; set; }
    public List<IFormFile> Images { get; set; } = null!;
}
