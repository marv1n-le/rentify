using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;
using MilkyShop.Repositories.Implement;
using MilkyShop.Repositories.Interface;

namespace MilkyShop.Repositories.Repository;

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