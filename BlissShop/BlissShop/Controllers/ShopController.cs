using BlissShop.Abstraction.Shop;
using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests.ShopAvatar;
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

        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddShopAsync(CreateShopDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.AddShopAsync(userId, dto);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteShopAsync(Guid id)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _shopService.DeleteShopAsync(sellerId, id);

            return result ? NoContent() : NotFound();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetShopByIdAsync(Guid id)
        {
            var result = await _shopService.GetShopByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetShopsForSellerAsync()
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.GetShopsForSellerAsync(userId);

            return Ok(result);
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateShopAsync(Guid shopId, UpdateShopDTO dto)
        {
            var result = await _shopService.UpdateShopAsync(shopId, dto);

            return Ok(result);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UploadAvatarAsync(UploadShopAvatarRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.UploadAvatarAsync(userId, request);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteAvatarAsync(DeleteShopAvatarRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.DeleteAvatarAsync(userId, request);

            return result ? NoContent() : NotFound();
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IsAprovedShop(Guid shopId, bool isAproved)
        {
            var result = await _shopService.AprovedShopAsync(shopId, isAproved);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Follow(Guid shopId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.FollowAsync(userId, shopId);

            return result ? Ok() : BadRequest();
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> Unfollow(Guid shopId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.UnfollowAsync(userId, shopId);
            
            return result ? NoContent() : NotFound();
        }
    }
}
