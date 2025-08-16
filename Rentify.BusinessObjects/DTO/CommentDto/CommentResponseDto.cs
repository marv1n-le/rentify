using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.BusinessObjects.DTO.CommentDto
{
    public class CommentResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? UserProfilePicture { get; set; }
    }
}
