using AutoMapper;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Abstraction.Shop;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Shop;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests.ShopAvatar;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BlissShop.BLL.Services;

public class ShopService : IShopService
{
    private readonly IRepository<ShopFollower> _shopFollowerRepository;
    private readonly IRepository<Shop> _shopRepository;
    private readonly UserManager<User> _userManager;
    private readonly ShopAvatarConfig _shopAvatarConfig;
    private readonly CallbackUrisConfig _callbackUrisConfig;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ShopService> _logger;
    private readonly IMapper _mapper;

    public ShopService(
        IRepository<ShopFollower> shopFollowerRepository,
        IRepository<Shop> shopRepository,
        UserManager<User> userManager,
        ShopAvatarConfig shopAvatarConfig,
        CallbackUrisConfig callbackUrisConfig,
        IEmailService emailService,
        IWebHostEnvironment env,
        ILogger<ShopService> logger,
        IMapper mapper)
    {
        _shopFollowerRepository = shopFollowerRepository;
        _shopRepository = shopRepository;
        _userManager = userManager;
        _shopAvatarConfig = shopAvatarConfig;
        _callbackUrisConfig = callbackUrisConfig;
        _emailService = emailService;
        _env = env;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ShopDTO> AddShopAsync(Guid userId, CreateShopDTO dto)
    {
        var shop = _mapper.Map<Shop>(dto);

        var usersInRole = await _userManager.GetUsersInRoleAsync("Admin");
        var admin = usersInRole.FirstOrDefault() 
            ?? throw new NotFoundException("Admin not found");

        shop.SellerId = userId;

        var result = await _shopRepository.InsertAsync(shop);

        if (!result)
            _logger.LogError("Shop was not created");

        var uri = string.Format(_callbackUrisConfig.AprovedShopUri, shop.Id);

        await _emailService.SendAsync(new AprovedShopMessage { Recipient = admin.Email!, Shop = shop, Uri = uri });

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
        if (!shop.AvatarName.IsNullOrEmpty())
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
            if (!entity!.AvatarName.IsNullOrEmpty())
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

        var contentPath = _env.ContentRootPath;
        var path = Path.Combine(contentPath, _shopAvatarConfig.Folder, request.ShopId.ToString());

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

        var uniqueSuffix = DateTime.UtcNow.Ticks;
        fileName = $"shop_{uniqueSuffix}{ext}";
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
        var path = Path.Combine(wwwPath, _shopAvatarConfig.Folder, request.ShopId.ToString());
        var file = Directory.GetFiles(path).FirstOrDefault(x => x.Contains(request.ShopId.ToString()));

        if (!File.Exists(file))
            throw new NotFoundException("File not found");

        File.Delete(file);

        shop.AvatarName = string.Empty;
        var result = await _shopRepository.UpdateAsync(shop);

        return result;
    }

    public async Task<bool> ApprovedShopAsync(Guid shopId, bool isAproved)
    {
        var shop = await _shopRepository
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == shopId)
            ?? throw new NotFoundException($"Shop not found with such id: {shopId}");

        var seller = await _userManager.FindByIdAsync(shop.SellerId.ToString())
            ?? throw new NotFoundException("Seller not found");

        if (isAproved)
        {
            shop.IsAproved = true;
            await _shopRepository.UpdateAsync(shop);
            await _emailService.SendAsync(new ShopIsAprovedMessage { Recipient = seller.Email!, Shop = shop });
        }
        else
        {
            await _shopRepository.DeleteAsync(shop);
            await _emailService.SendAsync(new ShopIsNotAprovedMessage { Recipient = seller.Email!, Shop = shop });
        }

        return true;
    }

    public async Task<bool> FollowAsync(Guid userId, Guid shopId)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == shopId)
            ?? throw new NotFoundException($"Shop not found with such id: {shopId}");

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        if (shop.SellerId == userId)
            throw new IncorrectParametersException("Seller can't follow his shop");

        var entity = await _shopFollowerRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.ShopId == shopId);

        if (entity != null)
            throw new IncorrectParametersException("User already follows this shop");

        entity = new ShopFollower
        {
            ShopId = shopId,
            UserId = userId
        };

        var result = await _shopFollowerRepository.InsertAsync(entity);

        if (!result)
            _logger.LogError("Failed to follow shop");

        return result;
    }

    public async Task<bool> UnfollowAsync(Guid userId, Guid shopId)
    {
        var entity = await _shopFollowerRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.ShopId == shopId)
            ?? throw new NotFoundException("Shop follower not found");

        var result = await _shopFollowerRepository.DeleteAsync(entity);

        if (!result)
            _logger.LogError("Failed to unfollow shop");

        return result;
    }
}
