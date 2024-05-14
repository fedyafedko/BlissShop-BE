using BlissShop.Abstraction.Auth;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.Configs;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Common.Requests;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services.Auth;

public class PasswordService : IPasswordService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly CallbackUrisConfig _callbackUrisConfig;
    private readonly ILogger<PasswordService> _logger;

    public PasswordService(
        UserManager<User> userManager,
        IEmailService emailService,
        CallbackUrisConfig callbackUrisConfig,
        ILogger<PasswordService> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _callbackUrisConfig = callbackUrisConfig;
        _logger = logger;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
           ?? throw new NotFoundException($"User with this email does not exist. Email: {request.Email}");

        if (!user.EmailConfirmed)
            throw new IncorrectParametersException("User's email is not confirmed");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var uri = string.Format(_callbackUrisConfig.ResetPasswordUri, user.Email, token);

        var emailSent = await _emailService.SendAsync( new ResetPasswordMessage { Recipient = request.Email, ResetPasswordUri = uri });

        return emailSent;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
                    ?? throw new NotFoundException($"Unable to find user by specified email. Email: {request.Email}");

        var isSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);

        if (isSamePassword)
            throw new IncorrectParametersException("New password have to differ from the old one");

        var result = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);

        if (!result.Succeeded)
        {
            _logger.LogIdentityErrors(user, result);
            throw new UserManagerException("Unable to reset password", result.Errors);
        }

        return result.Succeeded;
    }
}
