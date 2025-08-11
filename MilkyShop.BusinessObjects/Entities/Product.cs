using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class Product : BaseEntity
{
    public string? BrandId { get; set; }
    public string? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public virtual Brand? Brand { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}