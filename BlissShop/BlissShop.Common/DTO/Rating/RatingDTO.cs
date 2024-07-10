using BlissShop.Common.DTO.User;

namespace BlissShop.Common.DTO.Rating;

public class RatingDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public UserDTO User { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
    public double Rate { get; set; }
}