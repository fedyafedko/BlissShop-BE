using AutoMapper;
using BlissShop.Abstraction.Users;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.User;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
using BlissShop.Entities;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services.Users;

public class UserService : IUserService
{
    private readonly UserAvatarConfig _userAvatarConfig;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(UserAvatarConfig userAvatarConfig,
        UserManager<User> userManager,
        IWebHostEnvironment env,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userAvatarConfig = userAvatarConfig;
        _userManager = userManager;
        _env = env;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AvatarResponse> UploadAvatarAsync(Guid userId, IFormFile avatar)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var contetntPath = _env.ContentRootPath;
        var path = Path.Combine(contetntPath, _userAvatarConfig.Folder);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!string.IsNullOrEmpty(user.AvatarName))
        {
            var oldAvatarPath = Path.Combine(path, user.AvatarName);
            if (File.Exists(oldAvatarPath))
            {
                File.Delete(oldAvatarPath);
            }
        }

        var ext = Path.GetExtension(avatar.FileName);

        if (!_userAvatarConfig.FileExtensions.Contains(ext))
            throw new IncorrectParametersException("Invalid file extension");

        var newFileName = $"{userId}{ext}";
        var filePath = Path.Combine(path, newFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await avatar.CopyToAsync(stream);

        user.AvatarName = newFileName;
        await _userManager.UpdateAsync(user);

        var result = new AvatarResponse
        {
            Path = string.Format(_userAvatarConfig.Path, newFileName)
        };

        return result;
    }
    public async Task<bool> DeleteAvatarAsync(string avatar)
    {
        var wwwPath = _env.ContentRootPath;
        var path = Path.Combine(wwwPath, _userAvatarConfig.Folder, avatar);

        if (!File.Exists(path))
            throw new NotFoundException("File not found");

        await Task.Run(() => File.Delete(path));

        return true;
    }

    public async Task<UserDTO> Me(Guid userId)
    {
        var entity = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var file = Directory.GetFiles(Path.Combine(_env.ContentRootPath, _userAvatarConfig.Folder))
            .Select(x => Path.GetFileName(x))
            .FirstOrDefault(x => x.Contains(userId.ToString()));

        var role = await _userManager.GetRolesAsync(entity);

        var user = _mapper.Map<UserDTO>(entity);
        user.UrlAvatar = string.Format(_userAvatarConfig.Path, file);
        user.Role = role.FirstOrDefault()!;

        return user;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var changePassword = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!changePassword.Succeeded)
        {
            _logger.LogIdentityErrors(user, changePassword);
            throw new UserManagerException("User errors:", changePassword.Errors);
        }

        return true;
    }

    public async Task<UserDTO> EditProfile(Guid userId, UpdateUserDTO dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        _mapper.Map(dto, user);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogIdentityErrors(user, result);
            throw new UserManagerException("User errors:", result.Errors);
        }

        return _mapper.Map<UserDTO>(user);
    }
}