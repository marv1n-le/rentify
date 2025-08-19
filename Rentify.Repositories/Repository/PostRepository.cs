using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Helper;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<Post>> GetAllPost(SearchFilterPostDto searchFilterPostDto)
        {
            var query = _dbSet.AsNoTracking()
                .Include(p => p.User)
                .ThenInclude(u => u.Role)
                .Include(r => r.Inquiries)
                .ApplySearchFilter(searchFilterPostDto);

            var resultList = await query
                .OrderBy(x => Guid.NewGuid())
                .Include(p => p.User).ThenInclude(u => u.Role)
                .Skip((searchFilterPostDto.PageIndex - 1) * searchFilterPostDto.PageSize)
                .Take(searchFilterPostDto.PageSize)
                .ToListAsync();

            return resultList;
        }

        public Task<List<Post>> GetAllPost()
        {
            throw new NotImplementedException();
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
