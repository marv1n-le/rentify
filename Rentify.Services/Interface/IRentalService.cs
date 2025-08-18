using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface
{
    public interface IRentalService
    {
        Task<List<Rental>> GetAllRental();
        Task<Rental> GetRentalById(string postId);
        Task<string> CreateRental(RentalCreateDTO request);
        Task UpdateRental(RentalUpdateDTO request);
        Task DeleteRental(string postId);
    }
}