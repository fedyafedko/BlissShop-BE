using BlissShop.Abstraction.Users;
using BlissShop.Common.Configs;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BlissShop.BLL.Services.Users;

public class UserService : IUserService
{
    private readonly AvatarConfig _avatarConfig;
    private readonly IWebHostEnvironment _env;

    public UserService(AvatarConfig avatarConfig, IWebHostEnvironment env)
    {
        _avatarConfig = avatarConfig;
        _env = env;
    }

    public async Task<AvatarResponse> UploadAvatarAsync(Guid userId, IFormFile avatar)
    {
        var contetntPath = _env.ContentRootPath;
        var path = Path.Combine(contetntPath, _avatarConfig.Folder);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var ext = Path.GetExtension(avatar.FileName);

        if (!_avatarConfig.FileExtensions.Contains(ext))
            throw new IncorrectParametersException("Invalid file extension");

        var newFileName = $"{userId}{ext}";
        var filePath = Path.Combine(path, newFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await avatar.CopyToAsync(stream);

        var result = new AvatarResponse
        {
            Path = string.Format(_avatarConfig.Path, newFileName)
        };

        return result;
    }
    public async Task<bool> DeleteAvatarAsync(string avatar)
    {
        var wwwPath = _env.ContentRootPath;
        var path = Path.Combine(wwwPath, _avatarConfig.Folder, avatar);

        if (!File.Exists(path))
            throw new NotFoundException("File not found");

        await Task.Run(() => File.Delete(path));

        return true;
    }

}
