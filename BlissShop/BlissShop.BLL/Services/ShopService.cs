﻿using AutoMapper;
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
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ShopService : IShopService
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Shop> _shopRepository;
    private readonly ShopAvatarConfig _shopAvatarConfig;
    private readonly CallbackUrisConfig _callbackUrisConfig;
    private readonly IEmailService _emailService;
    private readonly ILogger<ShopService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ShopService(
        UserManager<User> userManager,
        IRepository<Shop> shopRepository,
        ShopAvatarConfig shopAvatarConfig,
        IEmailService emailService,
        ILogger<ShopService> logger,
        IWebHostEnvironment env,
        IMapper mapper,
        CallbackUrisConfig callbackUrisConfig)
    {
        _userManager = userManager;
        _shopRepository = shopRepository;
        _shopAvatarConfig = shopAvatarConfig;
        _emailService = emailService;
        _logger = logger;
        _env = env;
        _mapper = mapper;
        _callbackUrisConfig = callbackUrisConfig;
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

    public async Task<bool> AprovedShopAsync(Guid shopId, bool isAproved)
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
}
