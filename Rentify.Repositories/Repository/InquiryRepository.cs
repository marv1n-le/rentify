using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class InquiryRepository : GenericRepository<Inquiry>, IInquiryRepository
{
    public InquiryRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
    {
    }

    public async Task<List<Inquiry>> GetAllInquiryAsync()
    {
        return await _dbSet.Include(x => x.User)
            .Include(x => x.Post).ThenInclude(x => x.Item)
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<Inquiry?> GetByIdAsync(string id)
    {
        return await _dbSet
            .Include(i => i.Post) 
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task UpdateStatusAsync(string id, InquiryStatus status)
    {
        var inquiry = await _dbSet.FirstOrDefaultAsync(i => i.Id == id);
        if (inquiry == null) throw new Exception("Inquiry not found");
        inquiry.Status = status;
        await UpdateAsync(inquiry);
    }
}
