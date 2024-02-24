using AutoMapper;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Extensions;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductDTO, Product>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => String.Join(',', src.Tags)));
        CreateMap<UpdateProductDTO, Product>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => String.Join(',', src.Tags)));
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.SplitToList(",")));
    }
}
