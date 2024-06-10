using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.DTO.Settings;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.EntityFrameworkCore;

namespace BlissShop.BLL.Services;
public class SettingService : ISettingService
{
    private readonly IRepository<Setting> _settingRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public SettingService(
        IRepository<Setting> settingRepository,
        IEmailService emailService,
        IMapper mapper)
    {
        _settingRepository = settingRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    public Task<bool> SendEmailSupport(SupportRequest request)
    {
        var emailMessage = new SupportMessage { Message = request };

        var result = _emailService.SendAsync(emailMessage, request.Email);
        
        return result;
    }
    public async Task<bool> UpdateSettingsAsync(Guid userId, UpdateSettingDTO dto)
    {
        var setting = await _settingRepository.FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new NotFoundException("Settings not found");

        setting = _mapper.Map(dto, setting);

        var result = await _settingRepository.UpdateAsync(setting);

        return result;
    }

    public async Task<SettingDTO> GetSettingsForUserAsync(Guid userId)
    {
        var setting = await _settingRepository.FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new NotFoundException("Settings not found");

        return _mapper.Map<SettingDTO>(setting);
    }
}
