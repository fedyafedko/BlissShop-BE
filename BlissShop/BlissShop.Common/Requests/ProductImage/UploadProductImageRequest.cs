using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.Requests.ProductImage;

public class UploadProductImageRequest
{
    public Guid ProductId { get; set; }
    public List<IFormFile> Images { get; set; } = null!;
}