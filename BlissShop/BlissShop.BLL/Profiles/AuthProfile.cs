using AutoMapper;
using BlissShop.Common.DTO.Auth;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
        CreateMap<SignInDTO, User>();
    }
}
