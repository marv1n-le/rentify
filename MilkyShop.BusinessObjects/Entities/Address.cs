using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class Address : BaseEntity
{
    public string? UserId { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Street { get; set; }
    public bool IsDefault { get; set; } = false;
    
    public virtual User? User { get; set; }
}