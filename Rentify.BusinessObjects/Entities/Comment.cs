using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class Comment : BaseEntity
{
    public string? UserId { get; set; }
    public string? PostId { get; set; }
    public string? Content { get; set; }

    public virtual User? User { get; set; }
    public virtual Post? Post { get; set; }
}