using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class Discount : BaseEntity
{
    public string? UserId { get; set; }
    public string? DiscountBatchId { get; set; }
    public string? Code { get; set; }
    public bool IsUsed { get; set; } = false;
    
    public virtual User? User { get; set; }
    public virtual DiscountBatch? DiscountBatch { get; set; }
}