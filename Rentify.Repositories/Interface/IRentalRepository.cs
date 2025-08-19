using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using System.Linq.Expressions;

namespace Rentify.Repositories.Interface
{
    public interface IRentalRepository : IGenericRepository<Rental>
    {
        Task<List<Rental>> GetAllRental();
        Task<List<Rental>> GetAllAsync(Expression<Func<Rental, bool>> predicate);
        Task<Rental> GetById(string postId);
    }
}
