using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Item>> GetAllItems()
    {
        return await _unitOfWork.ItemRepository.GetAllAsync();
    }
    
    public async Task<Item?> GetItemById(string id)
    {
        return await _unitOfWork.ItemRepository.GetByIdAsync(id);
    }

    public async Task<bool> CreateItem(Item item)
    {
        var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(item.Id);
        if (existingItem != null)
            throw new Exception($"Item with id: {item.Id} already exists.");
        await _unitOfWork.ItemRepository.InsertAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateItem(Item item)
    {
        var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(item.Id);
        if (existingItem == null)
            throw new Exception($"Item with id: {item.Id} does not exist.");

        _unitOfWork.ItemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteItem(string id)
    {
        var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);
        if (existingItem == null)
            throw new Exception($"Item with id: {id} does not exist.");

        await _unitOfWork.ItemRepository.SoftDeleteAsync(existingItem);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}