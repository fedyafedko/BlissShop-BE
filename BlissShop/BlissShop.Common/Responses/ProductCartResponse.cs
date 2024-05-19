namespace BlissShop.Common.Responses;

public class ProductCartResponse
{
    public List<ProductCartItemResponse> Products { get; set; } = null!;
    public decimal TotalPrice { get; set; }
}