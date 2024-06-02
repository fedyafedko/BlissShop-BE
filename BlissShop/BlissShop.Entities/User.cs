using Microsoft.AspNetCore.Identity;


namespace BlissShop.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string AvatarName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public List<Shop> Shops { get; set; } = null!;
    public List<ShopFollower> FollowerShops { get; set; } = new List<ShopFollower>();
    public List<Order> Orders { get; set; } = null!;
    public List<Address> Addresses { get; set; } = null!;
    public ProductCart ProductCart { get; set; } = null!;
    public Setting Setting { get; set; } = null!;
    public List<Rating> Ratings { get; set; } = null!;
}