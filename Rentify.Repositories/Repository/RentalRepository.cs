using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository
{
    public class RentalRepository : GenericRepository<Rental>, IRentalRepository
    {
        public RentalRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<Rental>> GetAllRental()
        {
            var resultList = await _dbSet.AsNoTracking()
                .Include(p => p.User).ThenInclude(u => u.Role)
                .ToListAsync();

            return resultList;
        }

        public async Task<Rental> GetById(string postId)
        {
            var result = await _dbSet
                .Include(p => p.User).ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return result;
        }
    }
}