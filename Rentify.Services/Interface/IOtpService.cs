namespace Rentify.Services.Interface;

public interface IOtpService
{
    Task GenerateAndSendAsync(string userId);          
    Task<bool> VerifyAsync(string email, string code);      
    Task ResendAsync(string email);     
}