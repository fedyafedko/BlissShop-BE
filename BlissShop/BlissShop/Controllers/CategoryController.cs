using BlissShop.Abstraction;
using BlissShop.Common.DTO.Category;
using BlissShop.Common.Requests;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory(CreateCategoryDTO dto)
        {
            var result = await _categoryService.AddCategoryAsync(dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return result ? NoContent() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryService.GetAllCategory();

            return Ok(result);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadAvatar(UploadCategoryAvatarRequest request)
        {
            var result = await _categoryService.UploadAvatarAsync(request);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAvatar(Guid categoryId)
        {
            var result = await _categoryService.DeleteAvatarAsync(categoryId);

            return result ? NoContent() : NotFound();
        }
    }
}
