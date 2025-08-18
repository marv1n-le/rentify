using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(RentifyDbContext context, IHttpContextAccessor accessor) : base(context, accessor)
    {
    }

    public async Task<User?> GetUserAccount(string email, string password)
    {
        var userAccount = await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsDeleted == false);
        return userAccount;
    }

    public async Task<User?> GetUserById(string id)
    {
        var user = await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false);
        return user;
    }
}