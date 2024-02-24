using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddProductAsync(CreateProductDTO dto)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.AddProductAsync(sellerId, dto);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteProductAsync(Guid id)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.DeleteProductAsync(sellerId, id);

            return result ? NoContent() : NotFound();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProductAsync(Guid productId, UpdateProductDTO dto)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.UpdateProductAsync(sellerId, productId, dto);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductByIdAsync(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductsForShopAsync(Guid shopId)
        {
            var result = await _productService.GetProductsForShopAsync(shopId);

            return Ok(result);
        }   
    }
}
