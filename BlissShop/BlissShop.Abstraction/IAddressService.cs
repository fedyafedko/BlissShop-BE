using BlissShop.Common.DTO.Address;

namespace BlissShop.Abstraction;

public interface IAddressService
{
    Task<AddressDTO> CreateAddressAsync(Guid userId, CreateAddressDTO dto);
    Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);
    Task<List<AddressDTO>> GetAddressesAsync(Guid userId);
}
