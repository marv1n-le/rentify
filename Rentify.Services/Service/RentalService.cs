using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<RentalService> _logger;

        public RentalService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper, ILogger<RentalService> logger)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateFromInquiryAsync(string inquiryId, RentalCreateDTO rentalDto)
        {
            var inquiry = await _unitOfWork.InquiryRepository.GetByIdAsync(inquiryId);
            if (inquiry == null) throw new Exception("Inquiry not found");
            if (inquiry.Status != InquiryStatus.Open) throw new Exception("Inquiry is not open for processing");

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Please log in first");

            var newRental = new Rental
            {
                UserId = userId,
                User = user,
                RentalDate = inquiry.StartDate?.ToUniversalTime() ?? rentalDto.RentalDate?.ToUniversalTime(),
                ReturnDate = inquiry.EndDate?.ToUniversalTime() ?? rentalDto.ReturnDate?.ToUniversalTime(),
                TotalAmount = 0,
                Status = RentalStatus.Quoted, 
                PaymentStatus = PaymentStatus.Pending
            };

            await _unitOfWork.RentalRepository.InsertAsync(newRental);

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(inquiry.Post?.ItemId);
            if (item == null) throw new Exception("Item not found for this inquiry");

            var rentalItem = new RentalItem
            {
                RentalId = newRental.Id,
                ItemId = item.Id,
                Quantity = inquiry.Quantity,
                Price = item.Price
            };

            if (!await CheckAvailabilityAsync(item.Id, newRental.RentalDate, newRental.ReturnDate, rentalItem.Quantity))
                throw new Exception("Item not available in this period");

            await _unitOfWork.RentalItemRepository.InsertAsync(rentalItem);
            newRental.RentalItems.Add(rentalItem);

            newRental.TotalAmount = await CalculateTotalAmountAsync(newRental.Id);

            // Gán Rental vào Inquiry (thay vì inquiry.RentalId)
            inquiry.Rental = newRental;
            await _unitOfWork.InquiryRepository.UpdateStatusAsync(inquiryId, InquiryStatus.Quoted);

            await _unitOfWork.SaveChangesAsync();
            return newRental.Id;
        }

        public async Task<string> CreateRental(RentalCreateDTO rentalDto)
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Please log in first");

            var newRental = new Rental
            {
                UserId = userId,
                User = user,
                RentalDate = rentalDto.RentalDate?.ToUniversalTime(),
                ReturnDate = rentalDto.ReturnDate?.ToUniversalTime(),
                TotalAmount = 0,
                Status = rentalDto.Status,
                PaymentStatus = rentalDto.PaymentStatus
            };

            await _unitOfWork.RentalRepository.InsertAsync(newRental);

            foreach (var itemDto in rentalDto.RentalItems)
            {
                var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null) continue;

                var rentalItem = new RentalItem
                {
                    RentalId = newRental.Id,
                    ItemId = itemDto.ItemId,
                    Quantity = itemDto.Quantity,
                    Price = item.Price
                };

                if (!await CheckAvailabilityAsync(itemDto.ItemId, newRental.RentalDate, newRental.ReturnDate, rentalItem.Quantity))
                    throw new Exception($"Item {itemDto.ItemId} not available");

                await _unitOfWork.RentalItemRepository.InsertAsync(rentalItem);
                newRental.RentalItems.Add(rentalItem);
            }

            newRental.TotalAmount = await CalculateTotalAmountAsync(newRental.Id);
            await _unitOfWork.SaveChangesAsync();
            return newRental.Id;
        }

        public async Task DeleteRental(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null) throw new Exception($"Rental not found");

            await _unitOfWork.RentalRepository.SoftDeleteAsync(rental);
        }

        public async Task UpdateRental(RentalUpdateDTO request)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(request.RentalId);
            if (rental == null) throw new Exception($"Rental not found");

            rental.RentalDate = request.RentalDate?.ToUniversalTime();
            rental.ReturnDate = request.ReturnDate?.ToUniversalTime();
            rental.Status = request.Status;
            rental.PaymentStatus = request.PaymentStatus;

            foreach (var itemDto in request.RentalItems)
            {
                var existingRi = rental.RentalItems.FirstOrDefault(ri => ri.ItemId == itemDto.ItemId);
                if (existingRi != null)
                {
                    existingRi.Quantity = itemDto.Quantity;
                    await _unitOfWork.RentalItemRepository.UpdateAsync(existingRi);
                }
            }

            rental.TotalAmount = await CalculateTotalAmountAsync(rental.Id);
            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateRental(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null || rental.Status != RentalStatus.Confirmed) throw new Exception("Cannot activate");

            foreach (var ri in rental.RentalItems)
            {
                var item = ri.Item;
                if (item.Quantity < ri.Quantity) throw new Exception("Insufficient quantity");

                item.Quantity -= ri.Quantity;
                item.Status = ItemStatus.Rented;
                await _unitOfWork.ItemRepository.UpdateAsync(item);
            }

            rental.Status = RentalStatus.Active;
            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ConfirmRental(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null || rental.Status != RentalStatus.Quoted) throw new Exception("Cannot confirm");

            foreach (var ri in rental.RentalItems)
            {
                if (!await CheckAvailabilityAsync(ri.ItemId, rental.RentalDate, rental.ReturnDate, ri.Quantity))
                    throw new Exception($"Item {ri.ItemId} not available");
            }

            rental.Status = RentalStatus.Confirmed;
            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CompleteRental(string rentalId, decimal damageFee = 0, decimal lateFee = 0)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null || rental.Status != RentalStatus.Active) throw new Exception("Cannot complete");

            foreach (var ri in rental.RentalItems)
            {
                var item = ri.Item;
                item.Quantity += ri.Quantity;
                item.Status = ItemStatus.Available;
                await _unitOfWork.ItemRepository.UpdateAsync(item);
            }

            rental.TotalAmount += damageFee + lateFee;
            rental.Status = RentalStatus.Completed;
            rental.PaymentStatus = PaymentStatus.Paid;
            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CancelRental(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null || rental.Status == RentalStatus.Completed) throw new Exception("Cannot cancel");

            if (rental.Status == RentalStatus.Active)
            {
                foreach (var ri in rental.RentalItems)
                {
                    var item = ri.Item;
                    item.Quantity += ri.Quantity;
                    item.Status = ItemStatus.Available;
                    await _unitOfWork.ItemRepository.UpdateAsync(item);
                }
            }

            rental.Status = RentalStatus.Cancelled;
            rental.PaymentStatus = PaymentStatus.Refunded;
            await _unitOfWork.RentalRepository.UpdateAsync(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateTotalAmountAsync(string rentalId)
        {
            var rental = await _unitOfWork.RentalRepository.GetById(rentalId);
            if (rental == null) return 0;

            var rentalDays = Math.Max(1, (int)Math.Ceiling((rental.ReturnDate - rental.RentalDate)?.TotalDays ?? 0));
            decimal total = 0;

            foreach (var ri in rental.RentalItems)
            {
                total += ri.Price * rentalDays * ri.Quantity;
            }

            return total;
        }

        private async Task<bool> CheckAvailabilityAsync(string itemId, DateTime? start, DateTime? end, int requestedQty)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (item == null || item.Quantity < requestedQty) return false;

            var overlappingRentals = await _unitOfWork.RentalRepository.GetAllAsync(r =>
                (r.Status == RentalStatus.Quoted || r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active) &&
                r.RentalDate < end && r.ReturnDate > start);

            int bookedQty = overlappingRentals
                .SelectMany(r => r.RentalItems)
                .Where(ri => ri.ItemId == itemId)
                .Sum(ri => ri.Quantity);

            return (item.Quantity - bookedQty) >= requestedQty;
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

        private string GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
            return userId;
        }
    }
}