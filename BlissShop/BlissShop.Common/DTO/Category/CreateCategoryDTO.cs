using Microsoft.AspNetCore.Http;

namespace BlissShop.Common.DTO.Category;

public class CreateCategoryDTO
{
    public string Name { get; set; } = string.Empty;
}
