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
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddShopAsync(CreateShopDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.AddShopAsync(userId, dto);

            return Ok(result);
        }

        [HttpDelete("[action]")]
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
        public async Task<IActionResult> GetShopsForSellerAsync()
        {
            var userId = HttpContext.GetUserId();
            var result = await _shopService.GetShopsForSellerAsync(userId);

            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateShopAsync(Guid shopId, UpdateShopDTO dto)
        {
            var result = await _shopService.UpdateShopAsync(shopId, dto);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadAvatarAsync(UploadShopAvatarRequest request)
        {
            var result = await _shopService.UploadAvatarAsync(request);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAvatarAsync(DeleteShopAvatarRequest request)
        {
            var result = await _shopService.DeleteAvatarAsync(request);

            return result ? NoContent() : NotFound();
        }
    }
}
