using Microsoft.AspNetCore.Http;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;
using MilkyShop.Repositories.Implement;
using MilkyShop.Repositories.Interface;

namespace MilkyShop.Repositories.Repository;

public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(MilkyShopDbContext context, IHttpContextAccessor accessor) 
        : base(context, accessor)
    {
    }
}