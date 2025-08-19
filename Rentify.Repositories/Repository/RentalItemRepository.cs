using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository
{
    public class RentalItemRepository : IRentalItemRepository
    {
        private readonly RentifyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RentalItemRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<RentalItem>> GetByRentalIdAsync(string rentalId)
        {
            return await _context.RentalItems
                .Where(ri => ri.RentalId == rentalId)
                .Include(ri => ri.Item)
                .ToListAsync();
        }

        public async Task UpdateQuantityAsync(string rentalId, string itemId, int newQuantity)
        {
            var rentalItem = await _context.RentalItems
                .FirstOrDefaultAsync(ri => ri.RentalId == rentalId && ri.ItemId == itemId);
            if (rentalItem != null)
            {
                rentalItem.Quantity = newQuantity;
                _context.RentalItems.Update(rentalItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
