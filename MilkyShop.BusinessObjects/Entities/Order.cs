using MilkyShop.BusinessObjects.Entities.Base;
using MilkyShop.BusinessObjects.Enum;

namespace MilkyShop.BusinessObjects.Entities;

public class Order : BaseEntity
{
    public string? UserId { get; set; }
    public string? AddressId { get; set; }
    public string? DiscountId { get; set; }
    public string? Code { get; set; }
    public string? TrackingCode { get; set; }
    public decimal TotalPrice { get; set; } = 0;
    public decimal DiscountPrice { get; set; } = 0;
    public decimal ProductPrice { get; set; } = 0;
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? ReceivedDate { get; set; }
    
    public virtual User User { get; set; }
    public virtual Address? Address { get; set; }
    public virtual Discount? Discount { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();
}