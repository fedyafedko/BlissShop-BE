using BlissShop.Common.DTO.Rating;

namespace BlissShop.Abstraction;

public interface IRatingService
{
    public Task<RatingDTO> AddRatingAsync(Guid userId, CreateRatingDTO dto);
    public Task<List<RatingDTO>> GetRatingForProductAsync(Guid productId);
    public Task<List<RatingDTO>> GetRatingForUserAsync(Guid userId);
}
