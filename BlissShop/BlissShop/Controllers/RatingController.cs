using BlissShop.Abstraction;
using BlissShop.Common.DTO.Rating;
using BlissShop.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddRating(CreateRatingDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.AddRatingAsync(userId, dto);

        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetRatingForProduct(Guid productId)
    {
        var result = await _ratingService.GetRatingForProductAsync(productId);

        return Ok(result);
    }

    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> GetRatingForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.GetRatingForUserAsync(userId);

        return Ok(result);
    }
}
