namespace Rentify.BusinessObjects.DTO.UserDto;

public class SystemUserCreateDto
{
    public string RoleId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfilePicture { get; set; }
}