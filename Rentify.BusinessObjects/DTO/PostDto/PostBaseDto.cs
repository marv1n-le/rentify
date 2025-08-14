namespace Rentify.BusinessObjects.DTO.PostDto
{
    public class PostBaseDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
    }
}
