using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.Requests;

public class UploadCategoryAvatarRequest
{
    public Guid CategoryId { get; set; }
    public IFormFile Avatar { get; set; } = null!;
}