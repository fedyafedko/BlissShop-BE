using AutoMapper;
using BlissShop.Common.DTO;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class OrderProfile: Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.BuyerId));
    }
}
