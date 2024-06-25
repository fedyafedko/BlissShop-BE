using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using BlissShop.Common.Requests.ProductImage;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Add product.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a product.</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDTO dto)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.AddProductAsync(sellerId, dto);

            return Ok(result);
        }

        /// <summary>
        /// Delete product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.DeleteProductAsync(sellerId, id);

            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Update product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a product.</returns>
        [HttpPut("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(Guid productId, UpdateProductDTO dto)
        {
            var sellerId = HttpContext.GetUserId();
            var result = await _productService.UpdateProductAsync(sellerId, productId, dto);

            return Ok(result);
        }

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> This endpoint returns a product.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Get products for shop.
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns> This endpoint returns products.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsForShop(Guid shopId)
        {
            var result = await _productService.GetProductsForShopAsync(shopId);

            return Ok(result);
        }

        /// <summary>
        /// Get products for category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns> This endpoint returns products.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsForCategory(Guid categoryId)
        {
            var result = await _productService.GetProductForCategoryAsync(categoryId);

            return Ok(result);
        }

        /// <summary>
        /// Upload images for product.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns images path.</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ProductImagesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImages(UploadProductImageRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productService.UploadImagesAsync(userId, request);

            return Ok(result);
        }

        /// <summary>
        /// Delete images for product.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImages(DeleteProductImageRequest request)
        {
            var userId = HttpContext.GetUserId();
            var result = await _productService.DeleteImagesAsync(userId, request);

            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Search products.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns products.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PageList<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchProduct([FromQuery] SearchProductRequest request)
        {
            var result = await _productService.SearchProductAsync(request);

            return Ok(result);
        }
    }
}
