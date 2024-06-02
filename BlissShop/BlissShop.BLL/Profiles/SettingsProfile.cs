using AutoMapper;
using BlissShop.Common.DTO.Settings;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class SettingsProfile : Profile
{
    public SettingsProfile()
    {
        CreateMap<Setting, SettingDTO>();
        CreateMap<UpdateSettingDTO, Setting>();
    }
}
