using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class Rental : BaseEntity
{
    public string? UserId { get; set; }
    public string? InquiryId { get; set; }
    public DateTime? RentalDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? TotalAmount { get; set; } = 0;
    public RentalStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    // Navigation properties
    public virtual Inquiry? Inquiry { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<RentalItem> RentalItems { get; set; } = new List<RentalItem>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
