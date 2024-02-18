using System.ComponentModel.DataAnnotations;

namespace BlissShop.Entities;

public abstract class EntityBase
{
    [Key]
    public Guid Id { get; set; }
}
