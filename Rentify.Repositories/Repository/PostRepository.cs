using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Repositories.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<Post>> GetAllPost()
        {
            var resultList = await _dbSet.AsNoTracking()
                .Include(p => p.User).ThenInclude(u => u.Role)
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
