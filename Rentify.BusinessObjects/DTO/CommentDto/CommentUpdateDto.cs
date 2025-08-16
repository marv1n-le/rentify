using System.ComponentModel.DataAnnotations;
using Rentify.BusinessObjects.DTO.CommentDto;

namespace Rentify.BusinessObjects.DTO.CommentDto
{
    public class UpdateCommentDTO : BaseCommentDto
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
