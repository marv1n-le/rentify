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

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(inquiry.Post?.ItemId);
            if (item == null) throw new Exception("Item not found for this inquiry");

            if (!await CheckAvailabilityAsync(item.Id, inquiry.StartDate, inquiry.EndDate, inquiry.Quantity))
                throw new Exception("Item not available in this period");

            var newRental = new Rental
            {
                UserId = userId,
                User = user,
                RentalDate = inquiry.StartDate?.ToUniversalTime() ?? rentalDto.RentalDate?.ToUniversalTime(),
                ReturnDate = inquiry.EndDate?.ToUniversalTime() ?? rentalDto.ReturnDate?.ToUniversalTime(),
                TotalAmount = 0,
                Status = RentalStatus.Quoted,
                PaymentStatus = PaymentStatus.Pending,
                RentalItems = new List<RentalItem>()
            };

            var rentalItem = new RentalItem
            {
                RentalId = newRental.Id,
                ItemId = item.Id,
                Item = item,
                Quantity = inquiry.Quantity,
                Price = item.Price
            };

            newRental.RentalItems.Add(rentalItem);

            newRental.TotalAmount = await CalculateTotalAmountAsync(newRental);

            await _unitOfWork.RentalRepository.InsertAsync(newRental);
            await _unitOfWork.RentalItemRepository.GetByRentalIdAsync(newRental.Id);
            inquiry.Rental = newRental;
            inquiry.Status = InquiryStatus.Quoted;
            if (item.RemainingQuantity - inquiry.Quantity < 0)
                throw new Exception("Not enough quantity");
            item.RemainingQuantity -= inquiry.Quantity;
            await _unitOfWork.InquiryRepository.UpdateAsync(inquiry);
            await _unitOfWork.SaveChangesAsync();
            return newRental.Id;
        }

        public async Task<string> CreateRental(RentalCreateDTO rentalDto)
        {
            if (!rentalDto.RentalItems.Any())
                throw new Exception("At least one RentalItem is required");

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
                PaymentStatus = rentalDto.PaymentStatus,
                RentalItems = new List<RentalItem>()
            };

            foreach (var itemDto in rentalDto.RentalItems)
            {
                var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null) throw new Exception($"Item {itemDto.ItemId} not found");

                if (!await CheckAvailabilityAsync(item.Id, newRental.RentalDate, newRental.ReturnDate, itemDto.Quantity))
                    throw new Exception($"Item {itemDto.ItemId} not available");

                var rentalItem = new RentalItem
                {
                    RentalId = newRental.Id,
                    ItemId = item.Id,
                    Item = item,
                    Quantity = itemDto.Quantity,
                    Price = item.Price
                };
                newRental.RentalItems.Add(rentalItem);
            }

            newRental.TotalAmount = await CalculateTotalAmountAsync(newRental);
            await _unitOfWork.RentalRepository.InsertAsync(newRental);
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
            rental.TotalAmount = await CalculateTotalAmountAsync(rental);

            foreach (var itemDto in request.RentalItems)
            {
                var existingRi = rental.RentalItems.FirstOrDefault(ri => ri.ItemId == itemDto.ItemId);
                if (existingRi != null)
                {
                    existingRi.Quantity = itemDto.Quantity;
                    await _unitOfWork.RentalRepository.UpdateAsync(rental);
                }
                else
                {
                    var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemDto.ItemId);
                    if (item == null) continue;

                    var newRi = new RentalItem
                    {
                        RentalId = rental.Id,
                        ItemId = itemDto.ItemId,
                        Quantity = itemDto.Quantity,
                        Price = item.Price
                    };

                    if (!await CheckAvailabilityAsync(itemDto.ItemId, rental.RentalDate, rental.ReturnDate, newRi.Quantity))
                        throw new Exception($"Item {itemDto.ItemId} not available");

                    rental.RentalItems.Add(newRi);
                    await _unitOfWork.RentalRepository.UpdateAsync(rental);
                }
            }

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
            rental.PaymentStatus = PaymentStatus.Paid;
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

            await _unitOfWork.RentalRepository.UpdateAsync(rental);

            if (rental.Inquiry != null)
            {
                rental.Inquiry.Status = InquiryStatus.ClosedAccepted;
                await _unitOfWork.InquiryRepository.UpdateAsync(rental.Inquiry);
            }

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

            if (rental.Inquiry != null)
            {
                rental.Inquiry.Status = InquiryStatus.ClosedRejected;
                await _unitOfWork.InquiryRepository.UpdateAsync(rental.Inquiry);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateTotalAmountAsync(Rental rental)
        {
            if (rental == null)
                throw new Exception("Rental cannot be null");

            if (rental.RentalDate == null || rental.ReturnDate == null)
                throw new Exception("RentalDate and ReturnDate must be specified");

            if (rental.ReturnDate < rental.RentalDate)
                throw new Exception("ReturnDate must be on or after RentalDate");

            if (!rental.RentalItems.Any())
                throw new Exception("Rental must have at least one RentalItem");

            var rentalDays = Math.Max(1, (int)Math.Ceiling((rental.ReturnDate.Value - rental.RentalDate.Value).TotalDays));
            decimal total = 0;

            foreach (var ri in rental.RentalItems)
            {
                total += ri.Price * rentalDays * ri.Quantity;
            }

            _logger.LogInformation($"Calculated TotalAmount for Rental {rental.Id}: {total} (Days: {rentalDays}, Items: {rental.RentalItems.Count})");
            return total;
        }

        private async Task<bool> CheckAvailabilityAsync(string itemId, DateTime? start, DateTime? end, int requestedQty)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (item == null || item.Quantity < requestedQty) return false;

            var overlappingRentals = await _unitOfWork.RentalRepository.GetAllAsync(r =>
                (r.Status == RentalStatus.Quoted || r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active) &&
                r.RentalDate <= end && r.ReturnDate >= start);

            int bookedQty = overlappingRentals
                .SelectMany(r => r.RentalItems)
                .Where(ri => ri.ItemId == itemId)
                .Sum(ri => ri.Quantity);

            return (item.Quantity - bookedQty) >= requestedQty;
        }

        public async Task<List<Rental>> GetAllRental()
        {
            var rentals = await _unitOfWork.RentalRepository.GetAllRental();
            return rentals ?? new List<Rental>();
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