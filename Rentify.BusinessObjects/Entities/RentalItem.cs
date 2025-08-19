namespace Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Entities.Base;

public class RentalItem
{
    public string? ItemId { get; set; }
    public string? RentalId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; } = 0;

    public virtual Item? Item { get; set; }
    public virtual Rental? Rental { get; set; }
}