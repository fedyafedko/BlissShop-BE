using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Shop : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAproved { get; set; }
    public string AvatarName { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public List<Product> Products { get; set; } = null!;
}
