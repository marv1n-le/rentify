namespace Rentify.BusinessObjects.DTO.UserDto;

public class UpdateUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime? BirthDate { get; set; }
}