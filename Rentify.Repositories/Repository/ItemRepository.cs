using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Repository;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(RentifyDbContext context
        , IHttpContextAccessor accessor) : base(context, accessor)
    {
    }
}