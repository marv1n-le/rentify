using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;
using Rentify.Services.ExternalService.Redis; // ICacheService

namespace Rentify.Services.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        private const string Resource = "role";
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

        public RoleService(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        private static string BuildAllKey(int version) => $"{Resource}:v{version}:all";
        private static string BuildByIdKey(int version, string id) => $"{Resource}:v{version}:{id}";

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            var version = await _cache.GetVersionAsync(Resource);
            var key = BuildAllKey(version);

            var cached = await _cache.GetAsync<IEnumerable<Role>>(key);
            if (cached is not null)
                return cached;

            var data = await _unitOfWork.RoleRepository.GetAllAsync();

            await _cache.SetAsync(key, data, DefaultTtl);
            return data;
        }

        public async Task<Role?> GetRoleById(string id)
        {
            var version = await _cache.GetVersionAsync(Resource);
            var key = BuildByIdKey(version, id);

            var cached = await _cache.GetAsync<Role?>(key);
            if (cached is not null)
                return cached;

            var entity = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (entity is not null)
                await _cache.SetAsync(key, entity, DefaultTtl);

            return entity;
        }

        public async Task<string> CreateRole(Role role)
        {
            await _unitOfWork.RoleRepository.InsertAsync(role);
            await _unitOfWork.SaveChangesAsync();

            await _cache.IncreaseVersionAsync(Resource, TimeSpan.FromDays(7));
            return role.Id;
        }

        public async Task UpdateRole(Role role)
        {
            await _unitOfWork.RoleRepository.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            await _cache.IncreaseVersionAsync(Resource, TimeSpan.FromDays(7));
        }
    }
}
