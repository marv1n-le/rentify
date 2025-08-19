using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface
{
    public interface IRentalService
    {
        Task<List<Rental>> GetAllRental();
        Task<Rental> GetRentalById(string rentalId);
        Task<string> CreateRental(RentalCreateDTO request);
        Task<string> CreateFromInquiryAsync(string inquiryId, RentalCreateDTO rentalDto);
        Task UpdateRental(RentalUpdateDTO request);
        Task DeleteRental(string rentalId);
        Task ConfirmRental(string rentalId);
        Task ActivateRental(string rentalId);
        Task CompleteRental(string rentalId, decimal damageFee = 0, decimal lateFee = 0);
        Task CancelRental(string rentalId);
    }
}