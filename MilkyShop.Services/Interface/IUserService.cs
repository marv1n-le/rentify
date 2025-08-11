using MilkyShop.BusinessObjects.Entities;

namespace MilkyShop.Services.Interface;

public interface IUserService
{
    Task<User?> GetUserAccount(string userName, string password);
    Task<User?> GetUserById(int id);
    Task<IEnumerable<User>> GetAllUsers();
    Task<string> CreateUser(User user);
    Task UpdateUser(User user);
}