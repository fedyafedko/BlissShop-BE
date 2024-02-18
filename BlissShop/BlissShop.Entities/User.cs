using Microsoft.AspNetCore.Identity;


namespace BlissShop.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public List<Shop> Shops { get; set; } = null!;
    public List<Order> Orders { get; set; } = null!;
    public List<Address> Addresses { get; set; } = null!;
}