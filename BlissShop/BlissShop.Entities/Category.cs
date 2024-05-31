using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new List<Product>();
}
