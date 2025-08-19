using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rentify.Repositories.Helper;
using Rentify.Repositories.Infrastructure;
using RestSharp;
using RestSharp.Authenticators;

namespace Rentify.Services.ExternalService.MailGun;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _emailSettings;

    public EmailSenderService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var options = new RestClientOptions(_emailSettings.ApiBaseUri)
        {
            Authenticator = new HttpBasicAuthenticator("api", _emailSettings.ApiKey)
        };

        var client = new RestClient(options);

        var request = new RestRequest($"/v3/{_emailSettings.Domain}/messages", Method.Post);
        request.AddParameter("from", _emailSettings.FromEmail);
        request.AddParameter("to", email);
        request.AddParameter("subject", subject);
        request.AddParameter("text", StripHtml(message));
        request.AddParameter("html", message);

        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.Content != null)
            {
                string errorMsg = response.Content;

                try
                {
                    using var doc = JsonDocument.Parse(response.Content);
                    if (doc.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        errorMsg = messageElement.GetString()!;
                    }
                }
                catch
                {
                }

                if (errorMsg != null)
                    throw new ErrorException(StatusCodes.Status500InternalServerError, ApiCodes.INTERNAL_SERVER_ERROR,
                        errorMsg);
            }
        }
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var noTag = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        return System.Net.WebUtility.HtmlDecode(noTag);
    }
}