using MessageSiteAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace MessageSiteAPI.Services
{
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration;
        private ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<bool> SendConfirmationEmailAsync(string email, string token)
        {
            _logger.LogInformation($"Sending email to {email} with token: {token}");
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("MessageSite", _configuration["EmailSettings:From"]));
            mailMessage.To.Add(new MailboxAddress(email, email));
            mailMessage.Subject = "MessageSite Email Confirmation";
            mailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Your confirmation code: {token}"
            };

            using (var smpt = new SmtpClient())
            {
                try
                {
                    smpt.Connect(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]), false);
                    smpt.Authenticate(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                    smpt.Send(mailMessage);
                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending email to {email}: {ex.Message}");
                   return Task.FromResult(false);
                }
                finally
                {
                    smpt.Disconnect(true);
                }
            }
        }
    }
}
