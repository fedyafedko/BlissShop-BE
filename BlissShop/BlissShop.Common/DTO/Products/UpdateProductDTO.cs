namespace BlissShop.Common.DTO.Products;

public class UpdateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = null!;
    public int Quantity { get; set; }
}