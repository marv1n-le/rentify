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
    
    public async Task<string> CreateUser(User user)
    {
        await _unitOfWork.UserRepository.InsertAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user.Id;
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
}