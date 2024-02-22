using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace BlissShop.Abstraction.Users;

public interface IUserService
{
    Task<AvatarResponse> UploadAvatarAsync(Guid userId, IFormFile avatar);
    Task<bool> DeleteAvatarAsync(string avatar);
}
