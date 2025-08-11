using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;
using MilkyShop.Repositories.Implement;
using MilkyShop.Repositories.Interface;

namespace MilkyShop.Repositories.Repository;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    public BrandRepository(MilkyShopDbContext context, IHttpContextAccessor accessor) 
        : base(context, accessor)
    {
    }
    
    public async Task<Brand?> GetBrandByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Name == name && !b.IsDeleted);
    }
}