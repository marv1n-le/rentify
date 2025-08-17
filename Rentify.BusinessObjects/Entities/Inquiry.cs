using System.ComponentModel.DataAnnotations;
using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class Inquiry : BaseEntity
{
    public string PostId { get; set; } = default!;
    public string? RentalId { get; set; }
    public string UserId { get; set; } = default!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.Open;
    
    // Navigation properties
    public virtual Rental? Rental { get; set; }
    public virtual Post Post { get; set; } = default!;
    public virtual User User { get; set; } = default!;
}