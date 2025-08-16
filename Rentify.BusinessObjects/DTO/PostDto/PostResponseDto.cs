using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.BusinessObjects.DTO.PostDto
{
    public class PostResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Images { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? UserProfilePicture { get; set; }
        public int CommentCount { get; set; }
    }
}
