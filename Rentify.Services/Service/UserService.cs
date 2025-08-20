using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;
using System.Security.Claims;

namespace Rentify.Services.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IOtpService _otpService;

    public UserService(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor,
        IOtpService otpService)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
        _otpService = otpService;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User?> GetUserById(string id)
    {
        return await _unitOfWork.UserRepository.GetUserById(id);
    }

    public async Task<string> CreateUser(UserRegisterDto dto)
    {
        var existingUser = await _unitOfWork.UserRepository.IsEntityExistsAsync(x => x.Email == dto.Email);
        if (existingUser) throw new Exception($"Username {dto.Email} already exists.");

        var userRole = await _unitOfWork.RoleRepository.FindAsync(r => r.Name == "User")
                       ?? throw new Exception("User role not found");

        var newUser = new User
        {
            Email = dto.Email,
            Password = dto.Password, 
            FullName = dto.FullName,
            BirthDate = dto.BirthDate.HasValue
                ? DateTime.SpecifyKind(dto.BirthDate.Value, DateTimeKind.Utc)
                : null,
            ProfilePicture = dto.ProfilePicture,
            RoleId = userRole.Id,
            IsVerify = false
        };

        await _unitOfWork.UserRepository.InsertAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        
        await _otpService.GenerateAndSendAsync(newUser.Id);

        return newUser.Id;
    }

    public async Task<bool> CreateSystemUser(SystemUserCreateDto dto)
    {
        var existingUser = await _unitOfWork.UserRepository.IsEntityExistsAsync(x => x.Email == dto.Email);
        if (existingUser)
            throw new Exception($"Username {dto.Email} already exists.");

        User newUser = new User
        {
            Email = dto.Email,
            Password = dto.Password,
            FullName = dto.FullName,
            ProfilePicture = dto.ProfilePicture,
            IsVerify = true,
            RoleId = dto.RoleId
        };

        await _unitOfWork.UserRepository.InsertAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task UpdateUser(User user)
    {
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // public async Task<bool> UpdateUserProfile(UpdateUserDto dto)
    // {
    //     
    // }
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

    public string GetCurrentUserId()
    {
        var userId = _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
        return userId;
    }

    public async Task<List<User>> GetAllUsersExceptCurrent()
    {
        var currentUserId = GetCurrentUserId();

        var userList = await _unitOfWork.UserRepository.GetAllAsync();
        userList = userList.Where(x => !x.IsDeleted && x.Id != currentUserId);

        return userList.ToList();
    }
}