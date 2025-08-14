using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class Category : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}