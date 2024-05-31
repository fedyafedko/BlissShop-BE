using AutoMapper;
using BlissShop.Common.DTO.Category;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDTO>();
        CreateMap<CreateCategoryDTO, Category>();
    }
}
