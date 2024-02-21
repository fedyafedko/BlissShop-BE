using AutoMapper;
using BlissShop.Common.DTO.Auth;
using BlissShop.Entities;
using Google.Apis.Auth;

namespace BlissShop.BLL.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
        CreateMap<SignInDTO, User>();
        CreateMap<GoogleJsonWebSignature.Payload, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
