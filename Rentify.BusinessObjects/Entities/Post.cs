using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class Post : BaseEntity
{
    public string? ItemId { get; set; }
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();

    public virtual Item? Item { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}