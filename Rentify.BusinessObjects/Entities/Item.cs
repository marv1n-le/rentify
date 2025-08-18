using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class Item : BaseEntity
{
    public string? UserId { get; set; }
    public string? CategoryId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int RemainingQuantity { get; set; }
    public ItemStatus Status { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Post? Post { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<RentalItem> RentalItems { get; set; } = new List<RentalItem>();
}