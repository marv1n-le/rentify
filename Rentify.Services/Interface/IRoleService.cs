using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRoles();
    Task<Role?> GetRoleById(string id);
    Task<string> CreateRole(Role role);
    Task UpdateRole(Role role);
}