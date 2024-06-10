using BlissShop.Abstraction;
using BlissShop.Common.DTO.Category;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Adding category.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a category.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCategory(CreateCategoryDTO dto)
        {
            var result = await _categoryService.AddCategoryAsync(dto);

            return Ok(result);
        }

        /// <summary>
        /// Delete category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <returns> This endpoint returns categories.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryService.GetAllCategory();

            return Ok(result);
        }

        /// <summary>
        /// Upload avatar for category.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns an avatar path.</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AvatarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadAvatar(UploadCategoryAvatarRequest request)
        {
            var result = await _categoryService.UploadAvatarAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Delete avatar for category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpDelete("[action]")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAvatar(Guid categoryId)
        {
            var result = await _categoryService.DeleteAvatarAsync(categoryId);

            return result ? NoContent() : NotFound();
        }
    }
}
