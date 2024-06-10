using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Extensions;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductCartController : ControllerBase
    {
        private readonly IProductCartService _productCartService;

        public ProductCartController(IProductCartService productCartService)
        {
            _productCartService = productCartService;
        }

        /// <summary>
        /// product cart for user.
        /// </summary>
        /// <returns> This endpoint returns product for product cart.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ProductCartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductCart()
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.GetProductCart(userId);

            return Ok(result);
        }

        /// <summary>
        /// Add product to cart.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToProductCart(AddProductCartDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.AddToProductCart(userId, dto);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Remove product from cart.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveFromProductCart([FromQuery] Guid productId)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productCartService.RemoveFromProductCart(userId, productId);

            return result ? NoContent() : NotFound();
        }
    }
}
