﻿using BlissShop.Abstraction;
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

    /// <summary>
    /// Add rating for product.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns> This endpoint returns a rating.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRating(CreateRatingDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.AddRatingAsync(userId, dto);

        return Ok(result);
    }

    /// <summary>
    /// Get rating for product.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns> This endpoint returns a rating.</returns>
    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(List<RatingDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRatingForProduct(Guid productId)
    {
        var result = await _ratingService.GetRatingForProductAsync(productId);

        return Ok(result);
    }

    /// <summary>
    /// Get rating for user.
    /// </summary>
    /// <returns> This endpoint returns a rating.</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType(typeof(List<RatingDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRatingForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.GetRatingForUserAsync(userId);

        return Ok(result);
    }
}
