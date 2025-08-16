using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserAccount(string userName, string password);
    Task<User?> GetUserById(string id);
}