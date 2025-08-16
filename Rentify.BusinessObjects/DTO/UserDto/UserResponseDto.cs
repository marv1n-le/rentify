namespace Rentify.BusinessObjects.DTO.UserDto;

public class UserResponseDto
{
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? RoleName { get; set; }
}