using BlissShop.Common.DTO.Category;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;

namespace BlissShop.Abstraction;

public interface ICategoryService
{
    Task<CategoryDTO> AddCategoryAsync(CreateCategoryDTO dto);
    Task<bool> DeleteAvatarAsync(Guid categoryId);
    Task<bool> DeleteCategoryAsync(Guid id);
    Task<List<CategoryDTO>> GetAllCategory();
    Task<AvatarResponse> UploadAvatarAsync(UploadCategoryAvatarRequest request);
}
