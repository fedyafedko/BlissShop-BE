namespace BlissShop.Common.DTO.Rating;

public class RatingDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; } = null!;
    public double Rate { get; set; }
}