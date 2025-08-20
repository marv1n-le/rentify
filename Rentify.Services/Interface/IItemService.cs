using Rentify.BusinessObjects.DTO.ItemDto;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface;

public interface IItemService
{
    Task<IEnumerable<Item>> GetAllItems();
    Task<Item?> GetItemById(string id);
    Task<bool> CreateItem(ItemCreateDto request);
    Task<bool> UpdateItem(ItemUpdateDto request);
    Task<bool> DeleteItem(string id);
}