using AutoMapper;
using BlissShop.Common.DTO.User;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>();
        CreateMap<UpdateUserDTO, User>();
    }
}
