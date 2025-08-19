using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class OtpRepository : GenericRepository<Otp>, IOtpRepository
{
    public OtpRepository(RentifyDbContext context, IHttpContextAccessor accessor) : base(context, accessor)
    {
    }
    
    public async Task<IEnumerable<Otp>> FindAllAsync(Expression<Func<Otp, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public void DeleteRange(IEnumerable<Otp> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}