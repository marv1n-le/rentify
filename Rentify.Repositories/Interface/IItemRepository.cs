using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<Item> GetItemByIdAsync(string itemId);
    Task<List<Item>> GetAllItemHasNoPost();
    Task<List<Item>> GetAllItemAsync();
}