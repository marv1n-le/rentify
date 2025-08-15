using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(id);
    }

    public async Task<string> CreateUser(UserRegisterDto dto)
    {
        var existingUser = await _unitOfWork.UserRepository.IsEntityExistsAsync(x => x.Username == dto.Username);
        if (existingUser)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }

        var userRole = await _unitOfWork.RoleRepository.FindAsync(r => r.Name == "User");
        if (userRole == null)
            throw new Exception("User role not found");

        User newUser = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            FullName = dto.FullName,
            ProfilePicture = dto.ProfilePicture,
            RoleId = userRole.Id,
        };

        await _unitOfWork.UserRepository.InsertAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return newUser.Id;
    }

    public async Task<bool> CreateSystemUser(SystemUserCreateDto dto)
    {
        var existingUser = await _unitOfWork.UserRepository.IsEntityExistsAsync(x => x.Username == dto.Username);
        if (existingUser)
            throw new Exception($"Username {dto.Username} already exists.");

        User newUser = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            FullName = dto.FullName,
            ProfilePicture = dto.ProfilePicture,
            RoleId = dto.RoleId
        };

        await _unitOfWork.UserRepository.InsertAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task UpdateUser(User user)
    {
        _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<User?> GetUserAccount(string username, string password)
    {
        return await _unitOfWork.UserRepository.GetUserAccount(username, password);
    }
    
    public async Task<bool> SoftDeleteUser(string id)
    {
        await _unitOfWork.UserRepository.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}