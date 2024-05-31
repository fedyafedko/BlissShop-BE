using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Category;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly CategoryAvatarConfig _categoryAvatarConfig;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        IRepository<Category> categoryRepository,
        IMapper mapper,
        IWebHostEnvironment env,
        CategoryAvatarConfig categoryAvatarConfig,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _env = env;
        _categoryAvatarConfig = categoryAvatarConfig;
        _logger = logger;
    }

    public async Task<CategoryDTO> AddCategoryAsync(CreateCategoryDTO dto)
    {
        var entity = await _categoryRepository.FirstOrDefaultAsync(x => x.Name == dto.Name);

        if (entity != null)
            throw new AlreadyExistsException("Category already exists");

        entity = _mapper.Map<Category>(dto);

        var result = await _categoryRepository.InsertAsync(entity);

        if (!result)
            _logger.LogError("Shop was not created");

        return _mapper.Map<CategoryDTO>(entity);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var entity = _categoryRepository.FirstOrDefault(x => x.Id == id)
            ?? throw new NotFoundException($"Shop not found with such id: {id}");

        await DeleteAvatarAsync(id);

        var result = await _categoryRepository.DeleteAsync(entity);

        return result;
    }

    public async Task<List<CategoryDTO>> GetAllCategory()
    {
        var categories = await _categoryRepository.ToListAsync();
        var result = _mapper.Map<List<CategoryDTO>>(categories);

        foreach (var category in result)
        {
            category.ImageUrl = await GetImagePath(category.Id);
        }

        return result;
    }

    public async Task<AvatarResponse> UploadAvatarAsync(UploadCategoryAvatarRequest request)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == request.CategoryId)
            ?? throw new NotFoundException($"Shop not found with such id: {request.CategoryId}");

        var contentPath = _env.ContentRootPath;
        var path = Path.Combine(contentPath, _categoryAvatarConfig.Folder, request.CategoryId.ToString());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!string.IsNullOrEmpty(category.ImageName))
        {
            var oldAvatarPath = Path.Combine(path, category.ImageName);
            if (File.Exists(oldAvatarPath))
            {
                File.Delete(oldAvatarPath);
            }
        }

        var fileName = request.Avatar.FileName;
        var ext = Path.GetExtension(fileName);

        if (!_categoryAvatarConfig.FileExtensions.Contains(ext))
            throw new IncorrectParametersException("Invalid file extension");

        var uniqueSuffix = DateTime.UtcNow.Ticks;
        fileName = $"category_{uniqueSuffix}{ext}";
        var filePath = Path.Combine(path, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await request.Avatar.CopyToAsync(stream);

        category.ImageName = fileName;
        await _categoryRepository.UpdateAsync(category);

        var result = new AvatarResponse
        {
            Path = string.Format(_categoryAvatarConfig.Path, category.Id, fileName)
        };

        return result;
    }

    public async Task<bool> DeleteAvatarAsync(Guid categoryId)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == categoryId)
            ?? throw new NotFoundException($"Shop not found with such id: {categoryId}");

        var wwwPath = _env.ContentRootPath;
        var path = Path.Combine(wwwPath, _categoryAvatarConfig.Folder, categoryId.ToString(), category.ImageName);

        if (!File.Exists(path))
            throw new NotFoundException("File not found");

        await Task.Run(() => File.Delete(path));

        return true;
    }

    private async Task<string> GetImagePath(Guid categoryId)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == categoryId)
            ?? throw new NotFoundException($"Shop not found with such id: {categoryId}");

        return string.Format(_categoryAvatarConfig.Path, category.Id, category.ImageName);
    }
}
