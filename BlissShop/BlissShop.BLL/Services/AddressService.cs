using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Common.DTO.Address;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class AddressService : IAddressService
{
    private readonly IRepository<Address> _addressRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AddressService> _logger;
    private readonly IMapper _mapper;

    public AddressService(
        IRepository<Address> addressRepository,
        UserManager<User> userManager,
        ILogger<AddressService> logger,
        IMapper mapper)
    {
        _addressRepository = addressRepository;
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AddressDTO> CreateAddressAsync(Guid userId, CreateAddressDTO dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var entity = _mapper.Map<Address>(dto);
        entity.UserId = user.Id;

        var result = await _addressRepository.InsertAsync(entity);


        if (!result)
        {
            _logger.LogError("Address not created");
            throw new IncorrectParametersException("Address not created");
        }

        return _mapper.Map<AddressDTO>(entity);
    }

    public async Task<List<AddressDTO>> GetAddressesAsync(Guid userId)
    {
        var addresses = await _addressRepository.Where(x => x.UserId == userId).ToListAsync();

        return _mapper.Map<List<AddressDTO>>(addresses);
    }

    public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _addressRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId)
            ?? throw new NotFoundException("Address not found");

        var result = await _addressRepository.DeleteAsync(address);

        if (!result)
        {
            _logger.LogError("Address not deleted");
            throw new IncorrectParametersException("Address not deleted");
        }

        return result;
    }
}
