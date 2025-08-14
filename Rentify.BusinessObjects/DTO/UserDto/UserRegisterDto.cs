namespace Rentify.BusinessObjects.DTO.UserDto;

public class UserRegisterDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfilePicture { get; set; }
}