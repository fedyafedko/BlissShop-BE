using AutoMapper;
using BlissShop.Common.DTO.Shop;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class ShopProfile : Profile
{
    public ShopProfile()
    {
        CreateMap<Shop, ShopDTO>();
        CreateMap<CreateShopDTO, Shop>();
        CreateMap<UpdateShopDTO, Shop>();
    }
}
