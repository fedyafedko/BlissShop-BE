using BlissShop.Abstraction.Shop;
using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests.ShopAvatar;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        /// <summary>
        /// Add a shop.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a shop.</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ShopDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddShop(CreateShopDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.AddShopAsync(userId, dto);

            return Ok(result);
        }

        /// <summary>
        /// Delete a shop.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShop(Guid id)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _shopService.DeleteShopAsync(sellerId, id);

            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Get shop by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> This endpoint returns a shop.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ShopDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetShopById(Guid id)
        {
            var result = await _shopService.GetShopByIdAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Get shops for seller.
        /// </summary>
        /// <returns> This endpoint returns shops.</returns>
        [HttpGet("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(List<ShopDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetShopsForSeller()
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.GetShopsForSellerAsync(userId);

            return Ok(result);
        }

        /// <summary>
        /// Update a shop.
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a shop.</returns>
        [HttpPut("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ShopDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShop(Guid shopId, UpdateShopDTO dto)
        {
            var result = await _shopService.UpdateShopAsync(shopId, dto);

            return Ok(result);
        }

        /// <summary>
        /// Upload avatar for shop.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns an image path.</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(AvatarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadAvatar(UploadShopAvatarRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.UploadAvatarAsync(userId, request);

            return Ok(result);
        }

        /// <summary>
        /// Delete avatar for shop.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAvatar(DeleteShopAvatarRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.DeleteAvatarAsync(userId, request);

            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Approved shop.
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isAproved"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPut("[action]")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IsApprovedShop(Guid shopId, bool isAproved)
        {
            var result = await _shopService.ApprovedShopAsync(shopId, isAproved);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Followed to shop.
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPost("[action]")]
        [Authorize]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Follow(Guid shopId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.FollowAsync(userId, shopId);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Unfollowed to shop.
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unfollow(Guid shopId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.UnfollowAsync(userId, shopId);
            
            return result ? NoContent() : NotFound();
        }
    }
}
