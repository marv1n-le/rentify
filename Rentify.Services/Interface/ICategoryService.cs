using Rentify.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Services.Interface;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(string id);
    Task<string> CreateCategory(Category category);
    Task UpdateCategory(Category category);
    Task SoftDeleteCategory(object id);
}
