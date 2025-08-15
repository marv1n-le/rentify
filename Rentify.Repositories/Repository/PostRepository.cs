using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<Post>> GetAllPost(int index, int pageSize)
        {
            var resultList = await _dbSet.AsNoTracking()
                .OrderBy(x => Guid.NewGuid())
                .Include(p => p.User).ThenInclude(u => u.Role)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return resultList;
        }

        public async Task<Post> GetById(string postId)
        {
            var result = await _dbSet
                .Include(p => p.User).ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return result;
        }
    }
}
