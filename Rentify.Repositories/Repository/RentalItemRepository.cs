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
        public Task<List<RentalItem>> GetByRentalIdAsync(string rentalId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateQuantityAsync(string rentalId, string itemId, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
