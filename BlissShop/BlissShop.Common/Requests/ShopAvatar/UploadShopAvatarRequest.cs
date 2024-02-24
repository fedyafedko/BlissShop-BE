using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.Requests.ShopAvatar;

public class UploadShopAvatarRequest
{
    public Guid UserId { get; set; }
    public Guid ShopId { get; set; }
    public IFormFile Avatar { get; set; } = null!;
}