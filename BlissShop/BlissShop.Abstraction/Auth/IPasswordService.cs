using BlissShop.Common.Requests;

namespace BlissShop.Abstraction.Auth;

public interface IPasswordService
{
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}
