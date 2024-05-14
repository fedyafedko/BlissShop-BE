using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Order : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid BuyerId { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(Address))]
    public Guid AddressId { get; set; }
    public int Quantity { get; set; }
    public bool IsPaid { get; set; }
    
    public Address Address { get; set; } = null!;
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
