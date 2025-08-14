using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class Feedback : BaseEntity
{
    public string? UserId { get; set; }
    public string? OrderItemId { get; set; }
    public string? Description { get; set; }
    public float Rated { get; set; } = 0;

    public virtual User? User { get; set; }
}