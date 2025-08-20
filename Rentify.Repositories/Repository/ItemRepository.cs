using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(RentifyDbContext context
        , IHttpContextAccessor accessor) : base(context, accessor)
    {
    }

    public async Task<Item> GetItemByIdAsync(string itemId)
    {
        return await _dbSet
            .Include(x => x.User)
            .Include(x => x.Category)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Item>> GetAllItemHasNoPost()
    {
        return await _dbSet
            .Include(x => x.User)
            .Include(x => x.Category)
            .Where(x => !x.IsDeleted && x.Post == null)
            .ToListAsync();
    }

    public async Task<List<Item>> GetAllItemAsync()
    {
        return await _dbSet
            .Include(x => x.User)
            .Include(x => x.Category)
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }
}