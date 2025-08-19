using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.RentalDTO
{
    public class RentalBaseDTO
    {
        public string? UserId { get; set; }
        public DateTime? RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public RentalStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<RentalItemDTO> RentalItems { get; set; } = [];
    }

    public class RentalItemDTO
    {
        public string? ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; } = 0;
    }
}