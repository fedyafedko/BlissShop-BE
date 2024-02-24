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
    private readonly FileConfig _fileConfig;
    private readonly ILogger<ShopService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ShopService(
        IRepository<Shop> shopRepository,
        FileConfig fileConfig,
        ILogger<ShopService> logger,
        IWebHostEnvironment env,
        IMapper mapper)
    {
        _shopRepository = shopRepository;
        _fileConfig = fileConfig;
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

        return _mapper.Map<ShopDTO>(shop);
    }

    public async Task<IEnumerable<ShopDTO>> GetShopsForSellerAsync(Guid sellerId)
    {
        var query = _shopRepository.Where(x => x.SellerId == sellerId);
        var shops = await query.ToListAsync();

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

    public async Task<AvatarResponse> UploadAvatarAsync(UploadShopAvatarRequest request)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == request.ShopId)
            ?? throw new NotFoundException($"Shop not found with such id: {request.ShopId}");

        if (shop.SellerId != request.UserId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var contetntPath = _env.ContentRootPath;
        var path = Path.Combine(contetntPath, _fileConfig.FolderForShopAvatar, request.ShopId.ToString());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fileName = request.Avatar.FileName;
        var ext = Path.GetExtension(fileName);

        if (!_fileConfig.FileExtensions.Contains(ext))
            throw new IncorrectParametersException("Invalid file extension");

        var filePath = Path.Combine(path, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await request.Avatar.CopyToAsync(stream);

        var result = new AvatarResponse
        {
            Path = string.Format(_fileConfig.Path, fileName)
        };

        return result;
    }

    public async Task<bool> DeleteAvatarAsync(DeleteShopAvatarRequest request)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == request.ShopId)
            ?? throw new NotFoundException($"Shop not found with such id: {request.ShopId}");

        if (shop.SellerId != request.UserId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var wwwPath = _env.ContentRootPath;
        var path = Path.Combine(wwwPath, _fileConfig.FolderForShopAvatar, request.ShopId.ToString(), request.Avatar);

        if (!File.Exists(path))
            throw new NotFoundException("File not found");

        await Task.Run(() => File.Delete(path));

        return true;
    }
}
