using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class Rating : EntityBase
{
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public string Comment { get; set; } = null!;
    public double Rate { get; set; }

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}
