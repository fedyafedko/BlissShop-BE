namespace BlissShop.Common.DTO.Rating;

public class CreateRatingDTO
{
    public Guid ProductId { get; set; }
    public string Comment { get; set; } = null!;
    public double Rate { get; set; }
}
