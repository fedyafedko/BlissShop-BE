using BlissShop.Common.DTO.Address;
using BlissShop.Common.DTO.Products;

namespace BlissShop.Common.DTO;

public class OrderDTO
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public ProductDTO Product { get; set; } = null!;
    public AddressDTO Address { get; set; } = null!;
    public int Quantity { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreateAt { get; set; }
}
