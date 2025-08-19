using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface
{
    public interface IRentalItemRepository
    {
        Task<List<RentalItem>> GetByRentalIdAsync(string rentalId);
        Task UpdateQuantityAsync(string rentalId, string itemId, int newQuantity);
    }
}
