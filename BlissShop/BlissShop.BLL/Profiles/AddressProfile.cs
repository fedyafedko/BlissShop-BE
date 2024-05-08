using AutoMapper;
using BlissShop.Common.DTO.Address;
using BlissShop.Entities;

namespace BlissShop.BLL.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressDTO>();
        CreateMap<CreateAddressDTO, Address>();
    }
}
