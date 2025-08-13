using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(id);
    }
    public async Task<string> CreateCategory(Category category)
    {
        await _unitOfWork.CategoryRepository.InsertAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Id;
    }

    public async Task DeleteCategory(object id)
    {
        _unitOfWork.CategoryRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategory(Category category)
    {
        _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
