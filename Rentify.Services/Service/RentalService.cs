using AutoMapper;
using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public RentalService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<string> CreateRental(RentalCreateDTO rental)
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Please log in first");

            Rental newRental = new Rental
            {
                UserId = userId,
                User = user,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                TotalAmount = rental.TotalAmount,
                Status = rental.Status,
                PaymentStatus = rental.PaymentStatus
            };

            await _unitOfWork.RentalRepository.InsertAsync(newRental);
            await _unitOfWork.SaveChangesAsync();
            return newRental.Id;
        }

        public async Task DeleteRental(string postId)
        {
            var post = await _unitOfWork.PostRepository.GetById(postId);

            if (post == null)
                throw new Exception($"Rental with id: {postId} has not found");

            await _unitOfWork.PostRepository.SoftDeleteAsync(post);
        }

        public async Task<List<Rental>> GetAllRental()
        {
            var rentals = await _unitOfWork.RentalRepository.GetAllRental();

            if (rentals == null)
                throw new Exception("Has no record for Rental");

            return rentals;
        }

        public async Task<Rental> GetRentalById(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);

            if (rental == null)
                throw new Exception($"Rental with id: {rentalId} has not found");

            return rental;
        }

        public async Task UpdateRental(RentalUpdateDTO request)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(request.RentalId);

            if (rental == null)
                throw new Exception($"Rental with id: {request.RentalId} has not found");

            _mapper.Map(rental, request);

            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
            return userId;
        }
    }
}