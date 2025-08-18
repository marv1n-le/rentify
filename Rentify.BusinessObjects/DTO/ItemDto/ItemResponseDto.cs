using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ItemDto;

public class ItemResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? CategoryId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public ItemStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties for display
    public string? CategoryName { get; set; }
    public string? UserName { get; set; }
}