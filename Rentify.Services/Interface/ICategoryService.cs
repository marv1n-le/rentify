using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(string id);
    Task<string> CreateCategory(Category category);
    Task UpdateCategory(Category category);
    Task SoftDeleteCategory(object id);
}
