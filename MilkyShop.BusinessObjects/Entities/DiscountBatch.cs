using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class DiscountBatch : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int DiscountValue { get; set; }
    public float MinimumOrderValue { get; set; }
    public float MaximumDiscountValue { get; set; }
    public bool IsAutoGenerate { get; set; } = false;
    
    public virtual ICollection<Discount> Discounts { get; set; } = [];
}