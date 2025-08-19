using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.UserDto.RentalDTO
{
    public class RentalBaseDTO
    {
        public string? UserId { get; set; }
        public DateTime? RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public RentalStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}