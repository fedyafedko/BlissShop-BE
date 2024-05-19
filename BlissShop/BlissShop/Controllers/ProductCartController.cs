using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCartController : ControllerBase
    {
        private readonly IProductCartService _productCartService;

        public ProductCartController(IProductCartService productCartService)
        {
            _productCartService = productCartService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductCart()
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.GetProductCart(userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddToProductCart(AddProductCartDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.AddToProductCart(userId, dto);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromProductCart([FromQuery] Guid productId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.RemoveFromProductCart(userId, productId);

            return Ok(result);
        }
    }
}
