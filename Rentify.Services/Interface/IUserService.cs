using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface;

public interface IUserService
{
    Task<User?> GetUserAccount(string userName, string password);
    Task<User?> GetUserById(string id);
    Task<IEnumerable<User>> GetAllUsers();
    Task<string> CreateUser(UserRegisterDto user);
    Task<bool> CreateSystemUser(SystemUserCreateDto user);
    Task UpdateUser(User user);
    Task<bool> SoftDeleteUser(string id);
    Task<string?> GetCurrentUserIdAsync();
}