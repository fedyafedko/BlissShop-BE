using BlissShop.Common.Extensions;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlissShop.Abstraction.Auth;

namespace BlissShop.BLL.Services.Auth;

public class EmailConfirmationService : IEmailConfirmationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<UserRegister> _userRegisterRepository;
    private readonly AuthConfig _authConfig;

    public EmailConfirmationService(
        UserManager<User> userManager,
        IRepository<UserRegister> userRegisterRepository,
        AuthConfig authConfig,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _userRegisterRepository = userRegisterRepository;
        _authConfig = authConfig;
        _tokenService = tokenService;
    }

    public async Task<AuthSuccessDTO> ConfirmEmailAsync(ConfirmEmailDTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null)
            throw new NotFoundException("User with this id does not exist");

        if (user.EmailConfirmed)
            throw new IncorrectParametersException("Email is already confirmed");

        var option = await CanConfirmEmailAsync(dto.UserId, dto.Code);

        if (option)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        return await _tokenService.GetAuthTokensAsync(user);
    }

    public async Task ResendConfirmationCodeAsync(Guid userId)
    {
        var user = await _userRegisterRepository.FirstOrDefaultAsync(r => r.UserId == userId);
        if (user is null)
            throw new NotFoundException("User with this id does not exist");

        var result = await RegenerateEmailConfirmationCodeAsync(userId);
    }

    public async Task<int> GenerateEmailCodeAsync(Guid userId)
    {
        var existingRegistration = await _userRegisterRepository.FirstOrDefaultAsync(r => r.UserId == userId);

        if (existingRegistration is not null)
            throw new AlreadyExistsException("You have already received a code");

        var code = _authConfig.ConfirmationCodeLenght.GenerateCode();
        await _userRegisterRepository.InsertAsync(new UserRegister
        {
            Code = code,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(_authConfig.ConfirmationCodeLifetime),
            IsCodeRegenerated = false,
            UserId = userId
        });

        return code;
    }

    private async Task<bool> CanConfirmEmailAsync(Guid userId, int code)
    {
        var registration = await _userRegisterRepository
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (registration is null)
            throw new NotFoundException("You have not requested email confirmation");

        if (registration.Code != code)
            throw new IncorrectParametersException("Confirmation code is invalid");

        if (registration.ExpiresAt < DateTimeOffset.UtcNow)
            throw new ExpiredException("Confirmation code has expired. Please request a new one");

        await _userRegisterRepository.DeleteAsync(registration);
        return true;
    }

    private async Task<int> RegenerateEmailConfirmationCodeAsync(Guid userId)
    {
        var registration = await _userRegisterRepository.FirstOrDefaultAsync(r => r.UserId == userId);
        if (registration is null)
            throw new NotFoundException("You have not requested email confirmation");

        if (registration.IsCodeRegenerated)
            throw new AlreadyExistsException("You have already requested a new code");

        registration.Code = _authConfig.ConfirmationCodeLenght.GenerateCode();
        registration.ExpiresAt = DateTimeOffset.UtcNow.Add(_authConfig.ConfirmationCodeLifetime);
        registration.IsCodeRegenerated = true;
        await _userRegisterRepository.UpdateAsync(registration);

        return registration.Code;
    }
}
