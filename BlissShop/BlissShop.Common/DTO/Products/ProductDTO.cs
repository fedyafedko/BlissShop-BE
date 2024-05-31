namespace BlissShop.Common.DTO.Products;

public class ProductDTO
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = null!;
    public int Quantity { get; set; }
    public List<string> ImagesPath { get; set; } = null!;
}