using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Identity;
public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _sendGridClient;


    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger, ISendGridClient sendGridClient)
    {
        _configuration = configuration;
        _logger = logger;
        _sendGridClient = sendGridClient;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_configuration.GetSection("SendGridKey:From").Value, _configuration.GetSection("SendGridKey:Name").Value),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);

        var response = await _sendGridClient.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}