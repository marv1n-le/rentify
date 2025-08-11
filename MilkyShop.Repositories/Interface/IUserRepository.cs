using MilkyShop.BusinessObjects.Entities;
using MilkyShop.Repositories.Implement;

namespace MilkyShop.Repositories.Interface;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserAccount(string userName, string password);
}