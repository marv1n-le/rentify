using AutoMapper;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryById(string id)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(id);
    }
    public async Task<string> CreateCategory(Category category)
    {
        await _unitOfWork.CategoryRepository.InsertAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Id;
    }

    public async Task SoftDeleteCategory(object id)
    {
        await _unitOfWork.CategoryRepository.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategory(Category category)
    {
        await _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
