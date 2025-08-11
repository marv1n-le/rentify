using MilkyShop.BusinessObjects.Entities.Base;

namespace MilkyShop.BusinessObjects.Entities;

public class Brand : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Country { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}