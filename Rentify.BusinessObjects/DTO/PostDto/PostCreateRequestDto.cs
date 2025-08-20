namespace Rentify.BusinessObjects.DTO.PostDto
{
    public class PostCreateRequestDto : PostBaseDto
    {
        public string? ItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
