using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using System.Linq.Expressions;

namespace Rentify.Repositories.Repository
{
    public class RentalRepository : GenericRepository<Rental>, IRentalRepository
    {
        public RentalRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<Rental>> GetAllRental()
        {
            var resultList = await _dbSet
                .Include(p => p.User).ThenInclude(u => u.Role)
                .Include(p => p.RentalItems)
                //.ThenInclude(ri => ri.Item)
                .ToListAsync();

            //List<Rental> resultList = new List<Rental>();

            return resultList;
        }

        public async Task<List<Rental>> GetAllAsync(Expression<Func<Rental, bool>> predicate)
        {
            return await _dbSet
                .Include(r => r.RentalItems)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Rental> GetById(string postId)
        {
            var result = await _dbSet
                .Include(p => p.User).ThenInclude(u => u.Role)
                .Include(p => p.RentalItems).ThenInclude(ri => ri.Item)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return result;
        }
    }
}