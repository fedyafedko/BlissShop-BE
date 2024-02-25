using AutoMapper;
using BlissShop.Abstraction.Shop;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests.ShopAvatar;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ShopService : IShopService
{
    private readonly IRepository<Shop> _shopRepository;
    private readonly ShopAvatarConfig _shopAvatarConfig;
    private readonly ILogger<ShopService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ShopService(
        IRepository<Shop> shopRepository,
        ShopAvatarConfig shopAvatarConfig,
        ILogger<ShopService> logger,
        IWebHostEnvironment env,
        IMapper mapper)
    {
        _shopRepository = shopRepository;
        _shopAvatarConfig = shopAvatarConfig;
        _logger = logger;
        _env = env;
        _mapper = mapper;
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

        var result = _mapper.Map<ShopDTO>(shop);
        result.Path = string.Format(_shopAvatarConfig.Path, shop.Id, shop.AvatarName);

        return result;
    }

    public async Task<List<ShopDTO>> GetShopsForSellerAsync(Guid sellerId)
    {
        var query = _shopRepository.Where(x => x.SellerId == sellerId);
        var shops = await query.ToListAsync();

        var result = _mapper.Map<List<ShopDTO>>(shops);

        foreach (var shop in result)
        {
            var entity = _shopRepository.FirstOrDefault(x => x.Id == shop.Id);
            shop.Path = string.Format(_shopAvatarConfig.Path, shop.Id, entity!.AvatarName);
        }

        return result;
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

    public async Task<AvatarResponse> UploadAvatarAsync(Guid userId, UploadShopAvatarRequest request)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == request.ShopId)
            ?? throw new NotFoundException($"Shop not found with such id: {request.ShopId}");

        if (shop.SellerId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var contetntPath = _env.ContentRootPath;
        var path = Path.Combine(contetntPath, _shopAvatarConfig.Folder, request.ShopId.ToString());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!string.IsNullOrEmpty(shop.AvatarName))
        {
            var oldAvatarPath = Path.Combine(path, shop.AvatarName);
            if (File.Exists(oldAvatarPath))
            {
                File.Delete(oldAvatarPath);
            }
        }

        var fileName = request.Avatar.FileName;
        var ext = Path.GetExtension(fileName);

        if (!_shopAvatarConfig.FileExtensions.Contains(ext))
            throw new IncorrectParametersException("Invalid file extension");

        var filePath = Path.Combine(path, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await request.Avatar.CopyToAsync(stream);

        shop.AvatarName = fileName;
        await _shopRepository.UpdateAsync(shop);

        var result = new AvatarResponse
        {
            Path = string.Format(_shopAvatarConfig.Path, shop.Id, fileName)
        };

        return result;
    }

    public async Task<bool> DeleteAvatarAsync(Guid userId, DeleteShopAvatarRequest request)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == request.ShopId)
            ?? throw new NotFoundException($"Shop not found with such id: {request.ShopId}");

        if (shop.SellerId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var wwwPath = _env.ContentRootPath;
        var path = Path.Combine(wwwPath, _shopAvatarConfig.Folder, request.ShopId.ToString(), request.Avatar);

        if (!File.Exists(path))
            throw new NotFoundException("File not found");

        await Task.Run(() => File.Delete(path));

        return true;
    }
}
