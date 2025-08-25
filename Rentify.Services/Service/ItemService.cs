using AutoMapper;
using Rentify.BusinessObjects.DTO.ItemDto;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, ICategoryService categoryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
        _categoryService = categoryService;
    }

    public async Task<IEnumerable<Item>> GetAllItems()
    {
        return await _unitOfWork.ItemRepository.GetAllItemAsync();
    }

    public async Task<Item?> GetItemById(string id)
    {
        return await _unitOfWork.ItemRepository.GetItemByIdAsync(id);
    }

    public async Task<List<Item>> GetAllItemHasNoPost()
    {
        return await _unitOfWork.ItemRepository.GetAllItemHasNoPost();
    }

    public async Task<bool> CreateItem(ItemCreateDto request)
    {
        var userId = _userService.GetCurrentUserId();

        var item = _mapper.Map<Item>(request);
        item.UserId = userId;
        item.Status = ItemStatus.Available;
        item.RemainingQuantity = request.Quantity;

        await _unitOfWork.ItemRepository.InsertAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateItem(ItemUpdateDto request)
    {
        var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(request.Id);
        if (existingItem == null)
            throw new Exception($"Item with id: {request.Id} does not exist.");

        _mapper.Map(request, existingItem);

        await _unitOfWork.ItemRepository.UpdateAsync(existingItem);
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