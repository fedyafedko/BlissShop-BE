using AutoMapper;
using BlissShop.Abstraction.Shop;
using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ShopService : IShopService
{
    private readonly IRepository<Shop> _shopRepository;
    private readonly ILogger<ShopService> _logger;
    private readonly IMapper _mapper;

    public ShopService(
        IRepository<Shop> shopRepository,
        ILogger<ShopService> logger,
        IMapper mapper)
    {
        _shopRepository = shopRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ShopDTO> AddShopAsync(Guid userId, CreateShopDTO dto)
    {
        var shop = _mapper.Map<Shop>(dto);

        shop.SellerId = userId;

        var result = await _shopRepository.InsertAsync(shop);

        if (!result)
            _logger.LogError("Shop was not created");

        return _mapper.Map<ShopDTO>(shop);
    }

    public async Task<bool> DeleteShopAsync(Guid sellerId, Guid id)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Shop not found with such id: {id}");

        if (shop.SellerId != sellerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var result = await _shopRepository.DeleteAsync(shop);

        if(!result)
            _logger.LogError("Shop was not deleted");

        return result;
    }

    public async Task<ShopDTO> GetShopByIdAsync(Guid id)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Shop not found with such id: {id}");

        return _mapper.Map<ShopDTO>(shop);
    }

    public async Task<IEnumerable<ShopDTO>> GetShopsForSellerAsync(Guid sellerId)
    {
        var entities = _shopRepository.Where(x => x.SellerId == sellerId);
        var shops = await entities.ToListAsync();

        return _mapper.Map<IEnumerable<ShopDTO>>(shops);
    }

    public async Task<ShopDTO> UpdateShopAsync(Guid shopId, UpdateShopDTO dto)
    {
        var shop = _shopRepository.FirstOrDefault(x => x.Id == shopId)
            ?? throw new NotFoundException($"Shop not found with such id: {shopId}");

        _mapper.Map(dto, shop);

        var result = await _shopRepository.UpdateAsync(shop);

        if (!result)
            _logger.LogError("Shop was not updated");

        return _mapper.Map<ShopDTO>(shop);
    }
}
