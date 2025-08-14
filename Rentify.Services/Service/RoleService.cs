using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Role>> GetAllRoles()
    {
        return await _unitOfWork.RoleRepository.GetAllAsync();
    }

    public async Task<Role?> GetRoleById(string id)
    {
        return await _unitOfWork.RoleRepository.GetByIdAsync(id);
    }

    public async Task<string> CreateRole(Role role)
    {
        await _unitOfWork.RoleRepository.InsertAsync(role);
        await _unitOfWork.SaveChangesAsync();
        return role.Id;
    }

    public async Task UpdateRole(Role role)
    {
        await _unitOfWork.RoleRepository.UpdateAsync(role);
        await _unitOfWork.SaveChangesAsync();
    }
}