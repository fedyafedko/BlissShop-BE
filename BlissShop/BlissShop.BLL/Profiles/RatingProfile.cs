using AutoMapper;
using BlissShop.Common.DTO.Rating;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class RatingProfile : Profile
{
    public RatingProfile()
    {
        CreateMap<Rating, RatingDTO>();
        CreateMap<CreateRatingDTO, Rating>();
    }
}
