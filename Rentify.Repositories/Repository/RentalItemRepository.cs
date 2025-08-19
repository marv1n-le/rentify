using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository
{
    public class RentalItemRepository : GenericRepository<RentalItem>, IRentalItemRepository
    {
        public RentalItemRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }
        public async Task<List<RentalItem>> GetByRentalIdAsync(string rentalId)
        {
            return await _dbSet.Where(ri => ri.RentalId == rentalId)
                .Include(ri => ri.Item)
                .ToListAsync();
        }

        public async Task UpdateQuantityAsync(string rentalId, string itemId, int newQuantity)
        {
            var rentalItem = await _dbSet.FirstOrDefaultAsync(ri => ri.RentalId == rentalId && ri.ItemId == itemId);
            if (rentalItem != null)
            {
                rentalItem.Quantity = newQuantity;
                await UpdateAsync(rentalItem);
            }
        }
    }
}
