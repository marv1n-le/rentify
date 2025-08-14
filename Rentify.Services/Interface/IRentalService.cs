using Rentify.BusinessObjects.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rentify.Services.Interface
{
    public interface IRentalService
    {
        IEnumerable<Rental> GetAll();
        Task<IEnumerable<Rental>> GetAllAsync();
        Rental GetById(object id);
        Task<Rental> GetByIdAsync(object id);
    }
}