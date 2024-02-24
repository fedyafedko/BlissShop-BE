namespace BlissShop.Common.Requests.ProductImage;

public class DeleteProductImageRequest
{
    public Guid ProductId { get; set; }
    public List<string> Images { get; set; } = null!;
}
