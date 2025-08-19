using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface;

public interface IItemService
{
    Task<IEnumerable<Item>> GetAllItems();
    Task<Item?> GetItemById(string id);
    Task<bool> CreateItem(Item item);
    Task<bool> UpdateItem(Item item);
    Task<bool> DeleteItem(string id);
}