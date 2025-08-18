using Rentify.BusinessObjects.Entities.Base;

namespace Rentify.BusinessObjects.Entities;

public class Otp : BaseEntity
{
    public string? UserId { get; set; }
    public string? Code { get; set; }
    public string? MetaData { get; set; }
    public DateTime ExpiredAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
}