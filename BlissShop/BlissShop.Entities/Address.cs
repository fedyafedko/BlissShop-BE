using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Address : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
