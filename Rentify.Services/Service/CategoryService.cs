using AutoMapper;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;
using Rentify.Services.ExternalService.Redis; 

namespace Rentify.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        
        private const string Resource = "category";
        
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }
        
        private static string BuildAllKey(int version) => $"{Resource}:v{version}:all";
        private static string BuildByIdKey(int version, string id) => $"{Resource}:v{version}:{id}";
        
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var version = await _cache.GetVersionAsync(Resource);
            var key = BuildAllKey(version);

            var cached = await _cache.GetAsync<IEnumerable<Category>>(key);
            if (cached is not null)
                return cached;

            var data = await _unitOfWork.CategoryRepository.GetAllAsync();

            await _cache.SetAsync(key, data, DefaultTtl);

            return data;
        }

        public async Task<Category?> GetCategoryById(string id)
        {
            var version = await _cache.GetVersionAsync(Resource);
            var key = BuildByIdKey(version, id);

            var cached = await _cache.GetAsync<Category?>(key);
            if (cached is not null)
                return cached;

            var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (entity is not null)
            {
                await _cache.SetAsync(key, entity, DefaultTtl);
            }

            return entity;
        }

        public async Task<string> CreateCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.InsertAsync(category);
            await _unitOfWork.SaveChangesAsync();

            await _cache.IncreaseVersionAsync(Resource, TimeSpan.FromDays(7));

            return category.Id;
        }

        public async Task UpdateCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            await _cache.IncreaseVersionAsync(Resource, TimeSpan.FromDays(7));
        }

        public async Task SoftDeleteCategory(object id)
        {
            await _unitOfWork.CategoryRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            await _cache.IncreaseVersionAsync(Resource, TimeSpan.FromDays(7));
        }
    }
}
