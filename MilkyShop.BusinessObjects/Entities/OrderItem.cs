using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class OrderItem : BaseEntity
{
    public string? OrderId { get; set; }
    public string? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Note { get; set; }
        
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; }
    
}