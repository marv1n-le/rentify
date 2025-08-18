using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class InquiryRepository : GenericRepository<Inquiry>, IInquiryRepository
{
    public InquiryRepository(RentifyDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
    {
    }
}
