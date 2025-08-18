using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface
{
    public interface IRentalRepository : IGenericRepository<Rental>
    {
        Task<List<Rental>> GetAllRental();
        Task<Rental> GetById(string postId);
    }
}
