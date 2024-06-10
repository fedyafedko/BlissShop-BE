using BlissShop.Common.DTO.Settings;
using BlissShop.Common.DTO.User;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace BlissShop.Abstraction.Users;

public interface IUserService
{
    Task<AvatarResponse> UploadAvatarAsync(Guid userId, IFormFile avatar);
    bool DeleteAvatarAsync(Guid userId);
    Task<UserDTO> Me(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<UserDTO> EditProfile(Guid userId, UpdateUserDTO dto);
}
