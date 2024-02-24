using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.Requests.ShopAvatar;

public class DeleteShopAvatarRequest
{
    public Guid UserId { get; set; }
    public Guid ShopId { get; set; }
    public string Avatar { get; set; } = null!;
}
