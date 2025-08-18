using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class User : BaseEntity
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? RoleId { get; set; }
    public bool IsVerify { get; set; } = false;

    public virtual Role? Role { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();
}