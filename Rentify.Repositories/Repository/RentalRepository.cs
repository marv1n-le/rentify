using Microsoft.AspNetCore.Http;
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
    public class RentalRepository : GenericRepository<Rental>, IRentalRepository
    {
        public RentalRepository(MilkyShopDbContext context, IHttpContextAccessor accessor) : base(context, accessor)
        {
        }

        IEnumerable<Rental> IRentalRepository.GetAll()
        {
            return base.GetAll();
        }

        async Task<IEnumerable<Rental>> IRentalRepository.GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        Rental IRentalRepository.GetById(object id)
        {
            return base.GetById(id);
        }

        async Task<Rental> IRentalRepository.GetByIdAsync(object id)
        {
            return await base.GetByIdAsync(id);
        }
    }
}