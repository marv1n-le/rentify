using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(MilkyShopDbContext context, IHttpContextAccessor accessor) : base(context, accessor)
    {
    }

    public async Task<User?> GetUserAccount(string userName, string password)
    {
        var userAccount = await _dbSet
            .FirstOrDefaultAsync(u => u.Username == userName && u.Password == password && u.IsDeleted == false);
        return userAccount;
    }
}