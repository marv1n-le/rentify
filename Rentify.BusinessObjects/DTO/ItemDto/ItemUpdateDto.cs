using Rentify.BusinessObjects.Enum;
using System.ComponentModel.DataAnnotations;

namespace Rentify.BusinessObjects.DTO.ItemDto;

public class ItemUpdateDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category ID là bắt buộc")]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
    [StringLength(50, ErrorMessage = "Mã sản phẩm không được vượt quá 50 ký tự")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá là bắt buộc")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public ItemStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}